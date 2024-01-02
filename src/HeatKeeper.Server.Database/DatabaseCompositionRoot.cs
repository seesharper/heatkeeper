using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using DbReader;
using HeatKeeper.Server.Database.Migrations;
using LightInject;
using Microsoft.Extensions.Configuration;
using HeatKeeper.Abstractions.Configuration;
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
                .RegisterScoped<IDbConnection>(f => f.GetInstance<DbConnection>())
                .RegisterSingleton<IDatabaseMigrator, DatabaseMigrator>()
                .RegisterSingleton(f => new ResourceBuilder().Build<ISqlProvider>());
        }


        private static DbConnection CreateConnection(IServiceFactory serviceFactory)
        {
            var configuration = serviceFactory.GetInstance<IConfiguration>();
            var connection = new SQLiteConnection(configuration.GetConnectionString());
            connection.Open();
            return connection;
        }
    }
}
