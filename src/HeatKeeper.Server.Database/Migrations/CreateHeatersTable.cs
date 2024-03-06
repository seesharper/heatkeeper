using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(9, Order = 1)]
public class CreateHeatersTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateHeatersTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateHeatersTable);
}
