using System;
using System.Data;
using HeatKeeper.Server.Locations;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Users;
using LightInject;

using Vibrant.InfluxDB.Client;
using HeatKeeper.Server.Zones;
using CQRS.Command.Abstractions;
using CQRS.Transactions;
using CQRS.LightInject;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Security;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server
{
    public class ServerCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterCommandHandlers()
                .RegisterQueryHandlers()
                .Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>))
                .Decorate<IDbConnection, ConnectionDecorator>()
                .RegisterConstructorDependency<Logger>((f, p) => f.GetInstance<LogFactory>()(p.Member.DeclaringType))
                .RegisterSingleton<IInfluxClient>(f => new InfluxClient(new Uri("http://influxdb:8086")))
                .RegisterSingleton<IPasswordManager, PasswordManager>()
                .RegisterSingleton<IPasswordPolicy, PasswordPolicy>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()
                .Decorate<ICommandHandler<RegisterUserCommand>, RegisterUserValidator>()
                .Decorate<ICommandHandler<ChangePasswordCommand>, ChangePasswordValidator>()
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedLocationCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedZoneCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedUserCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedInsertUserLocationCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(AuthorizedCommandHandler<>))
                .Decorate(typeof(IQueryHandler<,>), typeof(AuthorizedQueryHandler<,>));
        }
    }
}
