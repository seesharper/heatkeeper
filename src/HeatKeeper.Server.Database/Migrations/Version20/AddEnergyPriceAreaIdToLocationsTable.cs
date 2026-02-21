using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version20;

[AppliesToVersion(20, Order = 1)]
public class AddEnergyPriceAreaIdToLocationsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddEnergyPriceAreaIdToLocationsTable);
    }
}
