using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(11, Order = 1)]
public class AddDeadSensorColumnsToSensorsTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public AddDeadSensorColumnsToSensorsTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.AddDeadSensorColumnsToSensorsTable);
}