namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Patch("/api/sensors/{SensorId}/assignZone")]
public record AssignZoneToSensorCommand(long SensorId, long ZoneId) : PatchCommand;

public class AssignZoneToSensor(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<AssignZoneToSensorCommand>
{
    public async Task HandleAsync(AssignZoneToSensorCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.AssignZoneToSensor, command);
}
