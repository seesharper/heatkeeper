using System.Data.Common;
using CQRS.LightInject;
using CQRS.Transactions;
using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Authentication;

using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Programs.Api;
using HeatKeeper.Server.Schedules;
using HeatKeeper.Server.Schedules.Api;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Users.Api;
using HeatKeeper.Server.Validation;
using HeatKeeper.Server.Zones;

using LightInject;
using Microsoft.Extensions.Configuration;
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
                .RegisterValidators()
                .Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>), sr =>
                {
                    return sr.ServiceType.IsGenericType && sr.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) && sr.ImplementingType is not null && sr.ImplementingType.GetConstructors()[0].GetParameters().Any(p => p.ParameterType == typeof(IDbConnection));
                })
                .Decorate<DbConnection, DbConnectionDecorator>()
                .RegisterSingleton<IBootStrapper>(sf => new JanitorBootStrapper(sf), "JanitorBootStrapper")
                .RegisterSingleton<IBootStrapper, DatabaseBootStrapper>("DatabaseBootStrapper")
                .RegisterSingleton<IPasswordManager, PasswordManager>()
                .RegisterSingleton<IManagedMqttClient>(sf => CreateManagedMqttClient(sf.GetInstance<IConfiguration>()))
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()

                .Decorate<ICommandHandler<CreateLocationCommand>, ValidateCreateLocation>()
                .Decorate<ICommandHandler<UpdateLocationCommand>, ValidateUpdateLocation>()
                .Decorate<ICommandHandler<CreateZoneCommand>, ValidateCreateZone>()

                .Decorate(typeof(ICommandHandler<>), typeof(CommandValidator<>))

                .Decorate(typeof(ICommandHandler<>), typeof(AuthorizedCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(SetNoContentResultOnDeleteCommands<>))
                .Decorate(typeof(ICommandHandler<>), typeof(SetNoContentResultOnPatchCommands<>))
                .Decorate(typeof(ICommandHandler<>), typeof(SetCreatedResultOnCreateCommands<>))

                .Decorate(typeof(IQueryHandler<,>), typeof(AuthorizedQueryHandler<,>))                
                .Decorate<ICommandHandler<DeleteProgramCommand>, WhenProgramIsDeleted>()
                .Decorate<ICommandHandler<MeasurementCommand[]>, WhenMeasurementsAreInserted>()                
                .Decorate<ICommandHandler<CreateScheduleCommand>, WhenScheduleIsCreated>()
                .Decorate<ICommandHandler<UpdateScheduleCommand>, WhenScheduleIsUpdated>()
                .Decorate<ICommandHandler<DeleteScheduleCommand>, WhenScheduleIsDeleted>()
                .Decorate<ICommandHandler<SetZoneHeatingStatusCommand>, WhenSettingZoneHeatingStatus>()
                .Decorate<ICommandHandler<ActivateProgramCommand>, WhenActivatingProgram>();


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
