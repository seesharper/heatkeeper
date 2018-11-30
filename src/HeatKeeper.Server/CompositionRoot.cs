using System.Data;
using System.IO;
using DbReader;
using LightInject;
using Microsoft.Data.Sqlite;

namespace HeatKeeper.Server
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped(CreateConnection);
        }

        private IDbConnection CreateConnection(IServiceFactory factory)
        {
            var settings = factory.GetInstance<ISettings>();
            var pathToDatabase = Path.Combine(settings.PathToDatabase, "heatkeeper.db");
            SqliteConnection connection = new SqliteConnection($"data source = {pathToDatabase}");
            DbReaderOptions.WhenReading<long?>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i)=> (string)rd.GetValue(i));
            connection.Open();
            return connection;
        }
    }
}