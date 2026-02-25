using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version21;

[AppliesToVersion(21, Order = 1)]
public class AddSmartMeterSensorIdToLocationsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddSmartMeterSensorIdToLocationsTable);
    }
}
