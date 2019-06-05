using System.Data;
using LightInject;
using HeatKeeper.Abstractions.Transactions;
using ResourceReader;

namespace HeatKeeper.Server.Database
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry
                .RegisterScoped(CreateConnection)
                .RegisterSingleton<IDatabaseInitializer, DatabaseInitializer>()
                .RegisterSingleton<IDbConnectionFactory, SqliteConnectionFactory>()
                .RegisterSingleton<IConnectionStringProvider, SqliteConnectionStringProvider>()
                .RegisterSingleton<ISqlProvider>(f => new ResourceBuilder().Build<ISqlProvider>());
        }

        private IDbConnection CreateConnection(IServiceFactory factory)
        {
            var connectionFactory = factory.GetInstance<IDbConnectionFactory>();
            return connectionFactory.CreateConnection();
        }
    }
}