using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(7, Order = 1)]
public class CreateRefreshTokensTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateRefreshTokensTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateRefreshTokensTable);
}
