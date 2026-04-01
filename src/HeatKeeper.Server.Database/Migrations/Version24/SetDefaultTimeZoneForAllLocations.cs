using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version24;

[AppliesToVersion(24, Order = 2)]
public class SetDefaultTimeZoneForAllLocations(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.SetDefaultTimeZoneForAllLocations);
    }
}
