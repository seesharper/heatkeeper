using System.Data;
using HeatKeeper.Abstractions.Logging;

namespace HeatKeeper.Server.Database
{
    public class LoggedDbConnection : IDbConnection
    {
        private readonly IDbConnection dbConnection;
        private readonly Logger logger;

        public LoggedDbConnection(IDbConnection dbConnection, Logger logger)
        {
            this.dbConnection = dbConnection;
            this.logger = logger;
        }

        public string ConnectionString { get => dbConnection.ConnectionString; set => dbConnection.ConnectionString = value; }

        public int ConnectionTimeout => dbConnection.ConnectionTimeout;

        public string Database => dbConnection.Database;

        public ConnectionState State => dbConnection.State;

        public IDbTransaction BeginTransaction()
        {
            return dbConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return dbConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            dbConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            dbConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return dbConnection.CreateCommand();
        }

        public void Dispose()
        {
            dbConnection.Dispose();
        }

        public void Open()
        {
            dbConnection.Open();
        }
    }
}
