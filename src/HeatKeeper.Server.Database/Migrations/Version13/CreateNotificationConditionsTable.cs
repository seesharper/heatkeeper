using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations.Version13;

[AppliesToVersion(13, Order = 2)]
public class CreateNotificationConditionsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(sqlProvider.CreateNotificationConditionsTable);
}
