using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(6, Order = 1)]
public class AddLastSeenColumnToSensors : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public AddLastSeenColumnToSensors(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.AddLastSeenColumnToSensors);
}