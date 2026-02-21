using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version19;

[AppliesToVersion(19, Order = 1)]
public class AddFixedEnergyPriceToLocationsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddFixedEnergyPriceToLocationsTable);
    }
}
