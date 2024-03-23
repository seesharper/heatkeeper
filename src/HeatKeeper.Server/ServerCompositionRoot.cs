using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using CQRS.Command.Abstractions;
using CQRS.LightInject;
using CQRS.Query.Abstractions;
using CQRS.Transactions;
using DbReader;
using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authentication.RefreshTokens;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Export;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Mqtt;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
using InfluxDB.Client;
using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server
{
    public class ServerCompositionRoot : ICompositionRoot
    {
        static ServerCompositionRoot()
        {
            DbReaderOptions.WhenReading<long>().Use((rd, i) => rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i) => (string)rd.GetValue(i));
            DbReaderOptions.WhenReading<bool>().Use((rd, i) => rd.GetInt32(i) != 0);                        
        }

        public void Compose(IServiceRegistry serviceRegistry) => _ = serviceRegistry.RegisterCommandHandlers()
                .RegisterQueryHandlers()
                .Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>), sr =>
                {
                    return sr.ServiceType.IsGenericType && sr.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) && sr.ImplementingType is not null && sr.ImplementingType.GetConstructors()[0].GetParameters().Any(p => p.ParameterType == typeof(IDbConnection));
                })
                // .Decorate<IDbConnection>((sf, c) =>
                // {
                //     var logger = sf.GetInstance<LogFactory>().CreateLogger<LoggedDbConnection>();
                //     return new LoggedDbConnection(c, (message) => logger.Debug(message));
                // })
                .Decorate<DbConnection, DbConnectionDecorator>()
                .RegisterSingleton<IBootStrapper, InfluxDbBootStrapper>("InfluxDbBootStrapper")
                .RegisterSingleton<IBootStrapper>(sf => new JanitorBootStrapper(sf), "JanitorBootStrapper")
                .RegisterSingleton<IBootStrapper, DatabaseBootStrapper>("DatabaseBootStrapper")
                .RegisterSingleton<IPasswordManager, PasswordManager>()
                .RegisterSingleton<IManagedMqttClient>(sf => CreateManagedMqttClient(sf.GetInstance<IConfiguration>()))
                .RegisterSingleton<IPasswordPolicy, PasswordPolicy>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IRefreshTokenProvider, RefreshTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()
                .RegisterSingleton<ISystemClock, SystemClock>()
                .RegisterScoped<IInfluxDBClient>(f => CreateInfluxDbClient(f.GetInstance<IConfiguration>()))
                //.Decorate<ICommandHandler<ExportMeasurementsCommand>, CumulativeMeasurementsExporter>()
                .Decorate<ICommandHandler<UpdateUserCommand>, ValidatedUpdateUserCommandHandler>()
                .Decorate<ICommandHandler<RegisterUserCommand>, RegisterUserValidator>()
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedLocationCommandHandler<>))
                .Decorate<ICommandHandler<ChangePasswordCommand>, ChangePasswordValidator>()
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedZoneCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedUserCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedInsertUserLocationCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(AuthorizedCommandHandler<>))
                .Decorate(typeof(IQueryHandler<,>), typeof(AuthorizedQueryHandler<,>))
                .Decorate<ICommandHandler<DeleteUserCommand>, ValidatedDeleteUserCommandHandler>()
                .Decorate(typeof(ICommandHandler<>), typeof(MaintainDefaultZonesCommandHandler<>))
                .Decorate<ICommandHandler<DeleteScheduleCommand>, BeforeDeleteSchedule>()
                .Decorate<ICommandHandler<DeleteProgramCommand>, BeforeDeleteProgram>()
                .Decorate<ICommandHandler<ExportMeasurementsToInfluxDbCommand>, WhenMeasurementAreExported>()
                .Decorate<ICommandHandler<MeasurementCommand[]>, WhenMeasurementsAreInserted>()
                .Decorate(typeof(ICommandHandler<>), typeof(ValidateSchedule<>))
                .Decorate<ICommandHandler<CreateScheduleCommand>, WhenScheduleIsCreated>()
                .Decorate<ICommandHandler<UpdateScheduleCommand>, WhenScheduleIsUpdated>()
                .Decorate<ICommandHandler<DeleteScheduleCommand>, WhenScheduleIsDeleted>()
                .Decorate<ICommandHandler<SetZoneHeatingStatusCommand>, WhenSettingZoneHeatingStatus>()
                .Decorate<ICommandHandler<ActivateProgramCommand>, WhenActivatingProgram>();

        private InfluxDBClient CreateInfluxDbClient(IConfiguration configuration)
            => new InfluxDBClient(configuration.GetInfluxDbUrl(), configuration.GetInfluxDbApiKey());

        private static IManagedMqttClient CreateManagedMqttClient(IConfiguration configuration)
        {
            var mqttBrokerAddress = configuration.GetMqttBrokerAddress();
            var mqttBrokerUser = configuration.GetMqttBrokerUser();
            var mqttBrokerPassword = configuration.GetMqttBrokerPassword();

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("HeatKeeper")
                    .WithCredentials(mqttBrokerUser, mqttBrokerPassword)
                    .WithTcpServer(mqttBrokerAddress, 1883)
                    .Build())
                .Build();

            var managedClient = new MqttFactory().CreateManagedMqttClient();
            managedClient.StartAsync(options).Wait();
            return managedClient;
        }


        public static bool IsRunningInContainer()
        {
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        }
    }
}
