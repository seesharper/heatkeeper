using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version14;

[AppliesToVersion(14, Order = 1)]
public class AddLongitudeAndLatitudeToLocationsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddLongitudeAndLatitudeToLocationsTable);
    }
}