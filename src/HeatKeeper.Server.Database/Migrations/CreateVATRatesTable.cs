using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(11, Order = 1)]
public class CreateVATRatesTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(sqlProvider.CreateVATRatesTable);
}