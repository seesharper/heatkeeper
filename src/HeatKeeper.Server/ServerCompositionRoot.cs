using System;
using System.Data;
using System.Linq;
using CQRS.Command.Abstractions;
using CQRS.LightInject;
using CQRS.Query.Abstractions;
using CQRS.Transactions;
using DbReader;
using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Configuration;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Export;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
using InfluxDB.Client;
using LightInject;
using Microsoft.Extensions.Configuration;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server
{
    public class ServerCompositionRoot : ICompositionRoot
    {
        static ServerCompositionRoot()
        {
            // DbReaderOptions.WhenReading<long?>().Use((rd, i) => rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i) => rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i) => (string)rd.GetValue(i));
            DbReaderOptions.WhenReading<bool>().Use((rd, i) => rd.GetInt32(i) != 0);
            // DbReaderOptions.WhenReading<RetentionPolicy>().Use((dr, i) => (RetentionPolicy)dr.GetInt64(i));
            // DbReaderOptions.WhenReading<MeasurementType>().Use((dr, i) => (MeasurementType)dr.GetInt64(i));
        }

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterCommandHandlers()
                .RegisterQueryHandlers()
                .Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>), sr =>
                {
                    return sr.ServiceType.IsGenericType && sr.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) && sr.ImplementingType.GetConstructors()[0].GetParameters().Any(p => p.ParameterType == typeof(IDbConnection));
                })
                .Decorate<IDbConnection>((sf, c) =>
                {
                    var logger = sf.GetInstance<LogFactory>().CreateLogger<LoggedDbConnection>();
                    return new LoggedDbConnection(c, (message) => logger.Debug(message));
                })
                .Decorate<IDbConnection, ConnectionDecorator>()
                .RegisterConstructorDependency<Logger>((f, p) => f.GetInstance<LogFactory>()(p.Member.DeclaringType))
                .RegisterSingleton<IInfluxClient>(f => CreateInfluxClient())
                .RegisterSingleton<IBootStrapper, InfluxDbBootStrapper>()
                .RegisterSingleton<IPasswordManager, PasswordManager>()
                .RegisterSingleton<IPasswordPolicy, PasswordPolicy>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()
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
                .Decorate<ICommandHandler<MeasurementCommand>, MaintainLatestZoneMeasurementDecorator>()
                .Decorate<ICommandHandler<MeasurementCommand[]>, ExportMeasurementsDecorator>()
                .Decorate<ICommandHandler<DeleteScheduleCommand>, BeforeDeleteSchedule>()
                .Decorate<ICommandHandler<DeleteProgramCommand>, BeforeDeleteProgram>();
        }

        private InfluxDBClient CreateInfluxDbClient(IConfiguration configuration)
            => new InfluxDBClient(configuration.GetInfluxDbUrl(), configuration.GetInfluxDbApiKey());

        private static IInfluxClient CreateInfluxClient()
        {
            string url = IsRunningInContainer() ? "http://influxdb:8086" : "http://localhost:8086";
            return new InfluxClient(new Uri(url));
        }

        public static bool IsRunningInContainer()
        {
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        }
    }
}
