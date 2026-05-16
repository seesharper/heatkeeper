using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version27;

[AppliesToVersion(27, Order = 1)]
public class AddEnergyThresholdToChargersTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddEnergyThresholdToChargersTable);
    }
}
