using HeatKeeper.Server.Sensors;

namespace HeatKeeper.Server.Measurements;

public class WhenMeasurementsAreInserted(ICommandHandler<MeasurementCommand[]> handler, ICommandExecutor commandExecutor) : ICommandHandler<MeasurementCommand[]>
{
    public async Task HandleAsync(MeasurementCommand[] measurements, CancellationToken cancellationToken = default)
    {
        await commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(measurements.Select(mc => mc.SensorId)), cancellationToken);
        await handler.HandleAsync(measurements, cancellationToken);
        await commandExecutor.ExecuteAsync(new MaintainLatestZoneMeasurementCommand(measurements), cancellationToken);
        var measurementsGroupedByExternalSensorId = measurements.Select(mte => new { mte.SensorId, mte.Created }).GroupBy(mte => mte.SensorId);
        foreach (var group in measurementsGroupedByExternalSensorId)
        {
            var latestCreatedDate = group.OrderBy(cr => cr.Created).Last().Created;
            await commandExecutor.ExecuteAsync(new UpdateLastSeenOnSensorCommand(group.Key, latestCreatedDate), cancellationToken);
        }
    }
}
