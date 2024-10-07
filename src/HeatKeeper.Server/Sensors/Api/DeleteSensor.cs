namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Delete("/api/sensors/{sensorId}")]
public record DeleteSensorCommand(long SensorId) : DeleteCommand;

public class DeleteSensor(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteSensorCommand>
{
    public async Task HandleAsync(DeleteSensorCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.DeleteSensorMeasurements, command);
        await dbConnection.ExecuteAsync(sqlProvider.DeleteSensor, command);
    }
}