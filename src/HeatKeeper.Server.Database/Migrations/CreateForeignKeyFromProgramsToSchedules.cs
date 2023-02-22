using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 4)]
public class CreateForeignKeyFromProgramsToSchedules : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateForeignKeyFromProgramsToSchedules(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateForeignKeyFromProgramsToSchedules);
}