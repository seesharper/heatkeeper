using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.SmartMeter;

namespace HeatKeeper.Server.Measurements;

public class WhenMeasurementsAreInserted(ICommandHandler<MeasurementCommand[]> handler, ICommandExecutor commandExecutor, ISmartMeterReadingsCache smartMeterReadingsCache) : ICommandHandler<MeasurementCommand[]>
{
    public async Task HandleAsync(MeasurementCommand[] measurements, CancellationToken cancellationToken = default)
    {
        await commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(measurements.Select(mc => mc.SensorId)), cancellationToken);
        await handler.HandleAsync(measurements, cancellationToken);
        await commandExecutor.ExecuteAsync(new MaintainLatestZoneMeasurementCommand(measurements), cancellationToken);
        await commandExecutor.ExecuteAsync(new CalculateEnergyCostsCommand(measurements), cancellationToken);
        var measurementsGroupedByExternalSensorId = measurements.Select(mte => new { mte.SensorId, mte.Created }).GroupBy(mte => mte.SensorId);
        foreach (var group in measurementsGroupedByExternalSensorId)
        {
            var latestCreatedDate = group.OrderBy(cr => cr.Created).Last().Created;
            await commandExecutor.ExecuteAsync(new UpdateLastSeenOnSensorCommand(group.Key, latestCreatedDate), cancellationToken);
        }

        var measurementsGroupedByMeasurementType = measurements.GroupBy(mte => mte.MeasurementType);
        // if measuremen type is 5 to 12 ( smart meter ), invalidate the cache
        if (measurementsGroupedByMeasurementType.Any(g => (int)g.Key >= 5 && (int)g.Key <= 12))
        {
            smartMeterReadingsCache.Invalidate();
        }




    }
}
