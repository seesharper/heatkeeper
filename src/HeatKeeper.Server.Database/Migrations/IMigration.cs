using System.Data;

namespace HeatKeeper.Server.Database.Migrations
{
    public interface IMigration
    {
        void Migrate(IDbConnection dbConnection);
    }
}
