using System.Data;
using System.Data.SQLite;
using DbReader;
using HeatKeeper.Server.Database.Migrations;
using LightInject;
using ResourceReader;

namespace HeatKeeper.Server.Database
{
    public class DatabaseCompositionRoot : ICompositionRoot
    {
        static DatabaseCompositionRoot()
        {

        }


        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry
                .RegisterScoped(CreateConnection)
                .RegisterSingleton<IDatabaseMigrator, DatabaseMigrator>()
                .RegisterSingleton(f => new ResourceBuilder().Build<ISqlProvider>());
        }


        private static IDbConnection CreateConnection(IServiceFactory serviceFactory)
        {
            var configuration = serviceFactory.GetInstance<ApplicationConfiguration>();
            var connection = new SQLiteConnection(configuration.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
