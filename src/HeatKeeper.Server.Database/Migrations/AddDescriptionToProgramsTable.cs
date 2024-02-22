using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(8, Order = 1)]
public class AddDescriptionToProgramsTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public AddDescriptionToProgramsTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.AddDescriptionToProgramsTable);
}