using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.ZoneTemperatures;

[RequireReporterRole]
public record UpdateZoneTemperaturesCommand(MeasurementCommand[] Measurements);

public class UpdateZoneTemperaturesCommandHandler(
    IQueryExecutor queryExecutor,
    IDbConnection dbConnection,
    ISqlProvider sqlProvider) : ICommandHandler<UpdateZoneTemperaturesCommand>
{
    public async Task HandleAsync(UpdateZoneTemperaturesCommand command, CancellationToken cancellationToken = default)
    {
        var temperatureMeasurements = command.Measurements
            .Where(m => m.MeasurementType == MeasurementType.Temperature)
            .ToArray();

        if (temperatureMeasurements.Length == 0)
            return;

        // Cache zone lookups and track the latest Created time per zone+hour
        var zoneIdCache = new Dictionary<string, long?>();
        var zoneHourUpdates = new Dictionary<(long zoneId, DateTime hour), DateTime>();

        foreach (var measurement in temperatureMeasurements)
        {
            if (!zoneIdCache.TryGetValue(measurement.SensorId, out var zoneId))
            {
                zoneId = await queryExecutor.ExecuteAsync(
                    new ZoneByExternalSensorQuery(measurement.SensorId), cancellationToken);
                zoneIdCache[measurement.SensorId] = zoneId;
            }

            if (zoneId == null)
                continue;

            var hour = TruncateToHour(measurement.Created);
            var key = (zoneId.Value, hour);

            if (!zoneHourUpdates.TryGetValue(key, out var existingMax) || measurement.Created > existingMax)
                zoneHourUpdates[key] = measurement.Created;
        }

        foreach (var (key, lastUpdate) in zoneHourUpdates)
        {
            var (zoneId, hour) = key;

            var averageTemperature = await dbConnection.ExecuteScalarAsync<double>(
                sqlProvider.GetAverageZoneTemperatureForHour,
                new { ZoneId = zoneId, HourStart = hour, HourEnd = hour.AddHours(1) });

            await dbConnection.ExecuteAsync(
                sqlProvider.UpsertZoneTemperature,
                new { ZoneId = zoneId, Temperature = averageTemperature, Hour = hour, LastUpdate = lastUpdate });
        }
    }

    private static DateTime TruncateToHour(DateTime dt)
        => new(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Kind);
}
