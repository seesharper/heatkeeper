using System.Data;
using System.IO;
using DbReader;
using LightInject;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Database
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped(CreateConnection);
            serviceRegistry.RegisterSingleton<IDatabaseInitializer, DatabaseInitializer>();
        }

        private IDbConnection CreateConnection(IServiceFactory factory)
        {
            var configuration = factory.GetInstance<IConfiguration>();
            var connectionString = configuration["ConnectionString"];
            SqliteConnection connection = new SqliteConnection(connectionString);
            DbReaderOptions.WhenReading<long?>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i)=> (string)rd.GetValue(i));
            connection.Open();
            return connection;
        }
    }
}