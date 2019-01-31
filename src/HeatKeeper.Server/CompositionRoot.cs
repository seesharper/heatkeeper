using System;
using System.Data;
using System.Net.Http;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database.Transactions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Logging;
using HeatKeeper.Server.Mapping;
using HeatKeeper.Server.Users;
using LightInject;

using Vibrant.InfluxDB.Client;

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
                .RegisterSingleton<IAuthenticationManager, AuthenticationManager>()
                .RegisterSingleton<ITokenProvider, JwtTokenProvider>();
        }
    }
}