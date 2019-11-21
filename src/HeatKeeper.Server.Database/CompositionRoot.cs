using System.Data;
using System.Data.SQLite;
using DbReader;
using LightInject;
using ResourceReader;

namespace HeatKeeper.Server.Database
{
    public class CompositionRoot : ICompositionRoot
    {
        static CompositionRoot()
        {
            DbReaderOptions.WhenReading<long?>().Use((rd, i) => rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i) => rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i) => (string)rd.GetValue(i));
            DbReaderOptions.WhenReading<bool>().Use((rd, i) => rd.GetInt32(i) == 0 ? false : true);
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
