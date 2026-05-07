namespace HeatKeeper.Server.Chargers.Api;

[RequireAdminRole]
[Patch("api/chargers/{chargerId}")]
public record UpdateChargerCommand(long ChargerId, long ZoneId, string Name, string Description, string MqttTopic, string OnPayload, string OffPayload) : PatchCommand;

public class UpdateCharger(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateChargerCommand>
{
    public async Task HandleAsync(UpdateChargerCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateCharger, command);
}
