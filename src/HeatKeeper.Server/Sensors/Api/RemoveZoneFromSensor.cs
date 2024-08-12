namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Patch("/api/sensors/{SensorId}/removeZone")]
public record RemoveZoneFromSensorCommand(long SensorId) : PatchCommand;

public class RemoveZoneFromSensor(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<RemoveZoneFromSensorCommand>
{
    public async Task HandleAsync(RemoveZoneFromSensorCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.RemoveZoneFromSensor, command);
}