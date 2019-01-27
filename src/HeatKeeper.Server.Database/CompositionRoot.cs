using System;
using System.Data;
using System.IO;
using DbReader;
using LightInject;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Database
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped(CreateConnection);
            serviceRegistry.RegisterSingleton<IDatabaseInitializer, DatabaseInitializer>();
            serviceRegistry.RegisterSingleton<ISqlProvider,SqlProvider>();
        }

        private IDbConnection CreateConnection(IServiceFactory factory)
        {
            var configuration = factory.GetInstance<IConfiguration>();
            var connectionString = configuration["ConnectionString"];
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Disposed += (a,e) => {
                Console.WriteLine("Disposed");
            };
            DbReaderOptions.WhenReading<long?>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i)=> (string)rd.GetValue(i));
            DbReaderOptions.WhenReading<bool>().Use((rd, i)=> rd.GetInt32(i) == 0 ? false : true);
            connection.Open();
            return connection;
        }
    }
}