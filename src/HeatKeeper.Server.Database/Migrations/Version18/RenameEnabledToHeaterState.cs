using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version18;

[AppliesToVersion(18, Order = 1)]
public class RenameEnabledToHeaterState(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.RenameEnabledToHeaterState);
    }
}
