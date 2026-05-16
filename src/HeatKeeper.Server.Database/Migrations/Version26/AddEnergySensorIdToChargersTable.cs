using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version26;

[AppliesToVersion(26, Order = 1)]
public class AddEnergySensorIdToChargersTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddEnergySensorIdToChargersTable);
    }
}
