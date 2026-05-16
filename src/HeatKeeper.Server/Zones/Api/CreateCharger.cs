using HeatKeeper.Server.Chargers;

namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Post("/api/zones/{ZoneId}/chargers")]
public record CreateChargerCommand(string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, long ZoneId, long? EnergySensorId = null, double EnergyThreshold = 0) : PostCommand;

public class CreateCharger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateChargerCommand>
{
    public async Task HandleAsync(CreateChargerCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertCharger, command);
}
