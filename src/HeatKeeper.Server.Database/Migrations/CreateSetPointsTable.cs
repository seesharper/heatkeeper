using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 3)]
public class CreateSetPointsTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateSetPointsTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateSetPointsTable);
}