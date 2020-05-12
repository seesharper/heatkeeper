using System;
using System.Data;

namespace HeatKeeper.Server.Database
{
    public class LoggedDbTransaction : IDbTransaction
    {
        private readonly IDbTransaction transaction;
        private readonly Action<string> logAction;

        public LoggedDbTransaction(IDbTransaction transaction, Action<string> logAction)
        {
            this.transaction = transaction;
            this.logAction = logAction;
        }

        public IDbConnection Connection => transaction.Connection;

        public IsolationLevel IsolationLevel => transaction.IsolationLevel;

        public void Commit()
        {
            logAction("Committing transaction");
            transaction.Commit();
        }

        public void Dispose()
        {
            logAction("Disposing transaction");
            transaction.Dispose();
        }

        public void Rollback()
        {
            logAction("Rolling back transaction");
            transaction.Rollback();
        }
    }
}
