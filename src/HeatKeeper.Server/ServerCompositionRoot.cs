using System;
using System.Data;
using System.Linq;
using CQRS.Command.Abstractions;
using CQRS.LightInject;
using CQRS.Query.Abstractions;
using CQRS.Transactions;
using DbReader;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
using LightInject;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server
{
    public class ServerCompositionRoot : ICompositionRoot
    {
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
                .RegisterSingleton<IPasswordManager, PasswordManager>()
                .RegisterSingleton<IPasswordPolicy, PasswordPolicy>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()
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
                .Decorate<ICommandHandler<MeasurementCommand[]>, ExportMeasurementsDecorator>();
        }

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
