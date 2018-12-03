using System.Data;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database.Transactions;
using HeatKeeper.Server.Logging;
using LightInject;

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
                .RegisterConstructorDependency<Logger>((f,p) => f.GetInstance<LogFactory>()(p.Member.DeclaringType));
        }
    }
}