using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 1)]
public class CreateProgramsTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateProgramsTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateProgramsTable);
}
