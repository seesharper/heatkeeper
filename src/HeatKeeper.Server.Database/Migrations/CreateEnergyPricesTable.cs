using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(11, Order = 3)]
public class CreateEnergyPricesTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(sqlProvider.CreateEnergyPricesTable);
}