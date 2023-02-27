using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 6)]
public class CreateForeignKeyFromLocationsToPrograms : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateForeignKeyFromLocationsToPrograms(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateForeignKeyFromLocationsToPrograms);
}