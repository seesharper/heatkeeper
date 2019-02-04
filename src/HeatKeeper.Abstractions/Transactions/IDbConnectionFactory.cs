using System.Data;

namespace HeatKeeper.Abstractions.Transactions
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}