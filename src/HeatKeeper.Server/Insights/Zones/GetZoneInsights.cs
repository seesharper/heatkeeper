using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Insights.Zones;

[RequireUserRole]
public record GetZoneInsightsQuery(long ZoneId, TimeRange Range) : IQuery<ZoneInsights>;

public class GetZoneInsightsQueryHandler(IQueryExecutor queryExecutor, TimeProvider timeProvider) : IQueryHandler<GetZoneInsightsQuery, ZoneInsights>
{
    public async Task<ZoneInsights> HandleAsync(GetZoneInsightsQuery query, CancellationToken cancellationToken = default)
    {
        DateTime since = query.Range switch
        {
            TimeRange.Hour => timeProvider.GetUtcNow().UtcDateTime.AddHours(-1),
            TimeRange.Day => timeProvider.GetUtcNow().UtcDateTime.AddDays(-1),
            TimeRange.Week => timeProvider.GetUtcNow().UtcDateTime.AddDays(-7),
            _ => throw new ArgumentOutOfRangeException(nameof(query.Range))
        };

        var zoneDetails = await queryExecutor.ExecuteAsync(new ZoneDetailsQuery(query.ZoneId), cancellationToken);
        var temperatureMeasurements = await queryExecutor.ExecuteAsync(new GetMeasurementsQuery(query.ZoneId, since, MeasurementType.Temperature), cancellationToken);
        var humidityMeasurements = await queryExecutor.ExecuteAsync(new GetMeasurementsQuery(query.ZoneId, since, MeasurementType.Humidity), cancellationToken);

        return new ZoneInsights(query.ZoneId, zoneDetails.Name, temperatureMeasurements, humidityMeasurements);
    }
}

public record ZoneInsights(long ZoneId, string Name, Measurement[] TemperatureMeasurements, Measurement[] HumidityMeasurements);

public enum TimeRange { Hour, Day, Week }
