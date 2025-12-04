using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version16;

[AppliesToVersion(16, Order = 1)]
public class AddDisabledReasonColumnToHeatersTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddDisabledReasonColumnToHeatersTable);
    }
}
