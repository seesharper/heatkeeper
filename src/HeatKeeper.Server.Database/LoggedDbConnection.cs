using System;
using System.Data;

namespace HeatKeeper.Server.Database
{
    public class LoggedDbConnection : IDbConnection
    {
        private readonly IDbConnection dbConnection;
        private readonly Action<string> logAction;

        public LoggedDbConnection(IDbConnection dbConnection, Action<string> logAction)
        {
            this.dbConnection = dbConnection;
            this.logAction = logAction;
        }

        public string ConnectionString { get => dbConnection.ConnectionString; set => dbConnection.ConnectionString = value; }

        public int ConnectionTimeout => dbConnection.ConnectionTimeout;

        public string Database => dbConnection.Database;

        public ConnectionState State => dbConnection.State;

        public IDbTransaction BeginTransaction()
        {
            logAction("Started transaction");
            return new LoggedDbTransaction(dbConnection.BeginTransaction(), logAction);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            logAction($"Started transaction with isolationlevel {il}");
            return dbConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            logAction($"Changing database to {databaseName}");
            dbConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            logAction("Closing connection");
            dbConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            logAction("Creating command");
            return dbConnection.CreateCommand();
        }

        public void Dispose()
        {
            logAction("Disposing connection");
            dbConnection.Dispose();
        }

        public void Open()
        {
            logAction("Opening the connection");
            dbConnection.Open();
        }
    }
}
