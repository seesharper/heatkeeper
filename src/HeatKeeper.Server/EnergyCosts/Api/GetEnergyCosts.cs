using HeatKeeper.Server.Locations;

namespace HeatKeeper.Server.EnergyCosts.Api;

[RequireUserRole]
[Get("api/energy-costs")]
public record GetEnergyCostsQuery(long LocationId, long? SensorId, TimePeriod TimePeriod, DateTime? FromDateTime, DateTime? ToDateTime) : IQuery<EnergyCostEntry[]>;

public record EnergyCostEntry(DateTime Timestamp, double PowerImport, decimal CostInLocalCurrency, decimal CostInLocalCurrencyAfterSubsidy, decimal CostInLocalCurrencyWithFixedPrice);

internal record LocationEnergyCostSettings(long? SmartMeterSensorId, EnergyCalculationStrategy EnergyCalculationStrategy);

public class GetEnergyCostsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<GetEnergyCostsQuery, EnergyCostEntry[]>
{
    public async Task<EnergyCostEntry[]> HandleAsync(GetEnergyCostsQuery query, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (fromDateTime, toDateTime) = GetTimeRange(query, now);
        var isHourly = IsHourlyResolution(query.TimePeriod, fromDateTime, toDateTime);

        if (query.SensorId.HasValue)
            return await QueryBySensor(query.SensorId.Value, fromDateTime, toDateTime, isHourly);

        var settings = (await dbConnection.ReadAsync<LocationEnergyCostSettings>(
            sqlProvider.GetLocationEnergyCostSettings,
            new { query.LocationId })).SingleOrDefault();

        if (settings?.EnergyCalculationStrategy == EnergyCalculationStrategy.SmartMeter)
        {
            if (settings.SmartMeterSensorId == null)
                return [];
            return await QueryBySensor(settings.SmartMeterSensorId.Value, fromDateTime, toDateTime, isHourly);
        }

        return await QueryByLocation(query.LocationId, fromDateTime, toDateTime, isHourly);
    }

    private async Task<EnergyCostEntry[]> QueryBySensor(long sensorId, DateTime from, DateTime to, bool isHourly)
    {
        var sql = isHourly ? sqlProvider.GetEnergyCostsByHourForSensor : sqlProvider.GetEnergyCostsByDayForSensor;
        return (await dbConnection.ReadAsync<EnergyCostEntry>(sql, new { SensorId = sensorId, FromDateTime = from, ToDateTime = to })).ToArray();
    }

    private async Task<EnergyCostEntry[]> QueryByLocation(long locationId, DateTime from, DateTime to, bool isHourly)
    {
        var sql = isHourly ? sqlProvider.GetEnergyCostsByHourForLocation : sqlProvider.GetEnergyCostsByDayForLocation;
        return (await dbConnection.ReadAsync<EnergyCostEntry>(sql, new { LocationId = locationId, FromDateTime = from, ToDateTime = to })).ToArray();
    }

    private static (DateTime from, DateTime to) GetTimeRange(GetEnergyCostsQuery query, DateTime now)
        => query.TimePeriod switch
        {
            TimePeriod.Today => (now.Date, now),
            TimePeriod.Yesterday => (now.Date.AddDays(-1), now.Date),
            TimePeriod.LastWeek => (now.Date.AddDays(-7), now.Date),
            TimePeriod.ThisMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now),
            TimePeriod.Custom => (query.FromDateTime!.Value, query.ToDateTime!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(query.TimePeriod))
        };

    private static bool IsHourlyResolution(TimePeriod timePeriod, DateTime from, DateTime to)
        => timePeriod switch
        {
            TimePeriod.Today => true,
            TimePeriod.Yesterday => true,
            TimePeriod.LastWeek => false,
            TimePeriod.ThisMonth => false,
            TimePeriod.Custom => (to - from).TotalHours <= 24,
            _ => throw new ArgumentOutOfRangeException(nameof(timePeriod))
        };
}
