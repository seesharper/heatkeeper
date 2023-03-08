using System.Data;
using DbReader;

namespace HeatKeeper.Server.Database.Migrations;

[AppliesToVersion(5, Order = 5)]
public class AddMqttTopicToZonesTable : IMigration
{
    private readonly ISqlProvider _sqlProvider;

    public AddMqttTopicToZonesTable(ISqlProvider sqlProvider)
        => _sqlProvider = sqlProvider;

    public void Migrate(IDbConnection dbConnection)
        => dbConnection.Execute(_sqlProvider.AddMqttTopicToZonesTable);
}