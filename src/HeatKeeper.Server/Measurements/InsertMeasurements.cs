namespace HeatKeeper.Server.Measurements;

public class InsertMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<MeasurementCommand[]>
{
    public async Task HandleAsync(MeasurementCommand[] commands, CancellationToken cancellationToken = default)
    {
        foreach (var command in commands)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertMeasurement, command);
        }
    }
}
