using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version15;

[AppliesToVersion(15, Order = 1)]
public class CreateEventTriggersTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.CreateEventTriggersTable);
    }
}