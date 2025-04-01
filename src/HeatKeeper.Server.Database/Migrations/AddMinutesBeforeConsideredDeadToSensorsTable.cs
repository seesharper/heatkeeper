using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(12, Order = 1)]
public class AddMinutesBeforeConsideredDeadToSensorsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(sqlProvider.AddMinutesBeforeConsideredDeadToSensorsTable);
}