using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations.Version13;

[AppliesToVersion(13, Order = 3)]
public class CreateNotificationSubscriptionsTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(sqlProvider.CreateNotificationSubscriptionsTable);
}
