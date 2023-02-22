using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 2)]
public class CreateSchedulesTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public CreateSchedulesTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.CreateSchedulesTable);

}
