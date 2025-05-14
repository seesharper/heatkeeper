namespace HeatKeeper.Server.Measurements;


[RequireReporterRole]
public record MeasurementCommand(
    string SensorId,
    MeasurementType MeasurementType,
    RetentionPolicy RetentionPolicy,
    double Value,
    DateTime Created);

// public record TestCommand : ArrayOf<MeasurementCommand>;


public class CreateMeasurements(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<MeasurementCommand[]>
{
    public async Task HandleAsync(MeasurementCommand[] commands, CancellationToken cancellationToken = default)
    {
        foreach (var command in commands)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertMeasurement, command);
        }
    }
}
