using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations
{
    [AppliesToVersion(2)]
    public class CreateLatestZoneMeasurementsTable : IMigration
    {
        private readonly ISqlProvider sqlProvider;

        public CreateLatestZoneMeasurementsTable(ISqlProvider sqlProvider)
        {
            this.sqlProvider = sqlProvider;
        }

        public void Migrate(IDbConnection dbConnection)
        {
            dbConnection.Execute(sqlProvider.CreateLatestZoneMeasurementsTable);
        }
    }
}
