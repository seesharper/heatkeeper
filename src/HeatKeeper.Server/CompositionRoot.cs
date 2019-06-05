using System;
using System.Data;
using System.Net.Http;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Abstractions.Transactions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Mapping;
using HeatKeeper.Server.Users;
using LightInject;

using Vibrant.InfluxDB.Client;
using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterCommandHandlers()
                .RegisterQueryHandlers()
                .Decorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandler<>))
                .Decorate<IDbConnection, ConnectionDecorator>()
                .RegisterConstructorDependency<Logger>((f,p) => f.GetInstance<LogFactory>()(p.Member.DeclaringType))
                .RegisterSingleton<IInfluxClient>(f => new InfluxClient(new Uri("http://influxdb:8086")))
                .RegisterSingleton<IMapper, Mapper>()
                .RegisterSingleton<IPasswordManager,PasswordManager>()
                .RegisterSingleton<IPasswordPolicy,PasswordPolicy>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>()
                .RegisterSingleton<IApiKeyProvider, ApiKeyProvider>()
                .RegisterSingleton<IEmailValidator, EmailValidator>()
                .Decorate<ICommandHandler<RegisterUserCommand>, RegisterUserValidator>()
                .Decorate<ICommandHandler<ChangePasswordCommand>, ChangePasswordValidator>()
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedLocationCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedZoneCommandHandler<>))
                .Decorate(typeof(ICommandHandler<>), typeof(ValidatedUserCommandHandler<>));
        }
    }
}