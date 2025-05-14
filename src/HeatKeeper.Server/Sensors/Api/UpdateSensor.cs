namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Patch("/api/sensors/{sensorId}")]
public record UpdateSensorCommand(long SensorId, string Name, string Description, string ExternalId, long MinutesBeforeConsideredDead) : PatchCommand;

public class UpdateSensor(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateSensorCommand>
{
    public async Task HandleAsync(UpdateSensorCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateSensor, command);
}


