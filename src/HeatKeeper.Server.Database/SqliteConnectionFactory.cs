using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using DbReader;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Abstractions.Transactions;

namespace HeatKeeper.Server.Database
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly Logger logger;

        static SqliteConnectionFactory()
        {
            DbReaderOptions.WhenReading<long?>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<long>().Use((rd, i)=> rd.GetInt32(i));
            DbReaderOptions.WhenReading<string>().Use((rd, i)=> (string)rd.GetValue(i));
            DbReaderOptions.WhenReading<bool>().Use((rd, i)=> rd.GetInt32(i) == 0 ? false : true);
        }

        public SqliteConnectionFactory(IConnectionStringProvider connectionStringProvider, Logger logger)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.logger = logger;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = connectionStringProvider.GetConnectionString();
            logger.Info($"Creating database connection using file {connectionString}");
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}