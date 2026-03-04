using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version22;

[AppliesToVersion(22, Order = 1)]
public class CreateZoneTemperaturesTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.CreateZoneTemperaturesTable);
    }
}
