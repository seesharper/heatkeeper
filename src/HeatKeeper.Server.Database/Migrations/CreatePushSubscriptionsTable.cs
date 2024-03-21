using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(10, Order = 1)]
public class CreatePushSubscriptionsTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreatePushSubscriptionsTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreatePushSubscriptionsTable);
}
