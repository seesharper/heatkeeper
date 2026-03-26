using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version23;

[AppliesToVersion(23, Order = 1)]
public class CreateLatestSensorMeasurementsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.CreateLatestSensorMeasurementsTable);
    }
}
