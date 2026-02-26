using HeatKeeper.Server.Locations;

namespace HeatKeeper.Server.EnergyCosts.Api;

[RequireUserRole]
[Get("api/energy-costs")]
public record GetEnergyCostsQuery(long LocationId, long? SensorId, TimePeriod TimePeriod) : IQuery<EnergyCostEntry[]>;

public record EnergyCostEntry(DateTime Timestamp, double PowerImport, decimal CostInLocalCurrency, decimal CostInLocalCurrencyAfterSubsidy, decimal CostInLocalCurrencyWithFixedPrice);

internal record LocationEnergyCostSettings(long? SmartMeterSensorId, EnergyCalculationStrategy EnergyCalculationStrategy);

public class GetEnergyCostsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<GetEnergyCostsQuery, EnergyCostEntry[]>
{
    public async Task<EnergyCostEntry[]> HandleAsync(GetEnergyCostsQuery query, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (fromDateTime, toDateTime) = GetTimeRange(query, now);
        var resolution = GetResolution(query.TimePeriod);

        if (query.SensorId.HasValue)
            return await QueryBySensor(query.SensorId.Value, fromDateTime, toDateTime, resolution);

        var settings = (await dbConnection.ReadAsync<LocationEnergyCostSettings>(
            sqlProvider.GetLocationEnergyCostSettings,
            new { query.LocationId })).SingleOrDefault();

        if (settings?.EnergyCalculationStrategy == EnergyCalculationStrategy.SmartMeter)
        {
            if (settings.SmartMeterSensorId == null)
                return [];
            return await QueryBySensor(settings.SmartMeterSensorId.Value, fromDateTime, toDateTime, resolution);
        }

        return await QueryByLocation(query.LocationId, fromDateTime, toDateTime, resolution);
    }

    private async Task<EnergyCostEntry[]> QueryBySensor(long sensorId, DateTime from, DateTime to, Resolution resolution)
    {
        var sql = resolution switch
        {
            Resolution.Hourly => sqlProvider.GetEnergyCostsByHourForSensor,
            Resolution.Daily => sqlProvider.GetEnergyCostsByDayForSensor,
            Resolution.Monthly => sqlProvider.GetEnergyCostsByMonthForSensor,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution))
        };
        return (await dbConnection.ReadAsync<EnergyCostEntry>(sql, new { SensorId = sensorId, FromDateTime = from, ToDateTime = to })).ToArray();
    }

    private async Task<EnergyCostEntry[]> QueryByLocation(long locationId, DateTime from, DateTime to, Resolution resolution)
    {
        var sql = resolution switch
        {
            Resolution.Hourly => sqlProvider.GetEnergyCostsByHourForLocation,
            Resolution.Daily => sqlProvider.GetEnergyCostsByDayForLocation,
            Resolution.Monthly => sqlProvider.GetEnergyCostsByMonthForLocation,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution))
        };
        return (await dbConnection.ReadAsync<EnergyCostEntry>(sql, new { LocationId = locationId, FromDateTime = from, ToDateTime = to })).ToArray();
    }

    private static (DateTime from, DateTime to) GetTimeRange(GetEnergyCostsQuery query, DateTime now)
        => query.TimePeriod switch
        {
            TimePeriod.Today => (now.Date, now),
            TimePeriod.Yesterday => (now.Date.AddDays(-1), now.Date),
            TimePeriod.LastWeek => (now.Date.AddDays(-7), now.Date),
            TimePeriod.ThisWeek => (now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7)), now),
            TimePeriod.ThisMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now),
            TimePeriod.LastMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1), new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)),
            TimePeriod.ThisYear => (new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), now),
            TimePeriod.LastYear => (new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            _ => throw new ArgumentOutOfRangeException(nameof(query.TimePeriod))
        };

    private static Resolution GetResolution(TimePeriod timePeriod)
        => timePeriod switch
        {
            TimePeriod.Today => Resolution.Hourly,
            TimePeriod.Yesterday => Resolution.Hourly,
            TimePeriod.LastWeek => Resolution.Daily,
            TimePeriod.ThisWeek => Resolution.Daily,
            TimePeriod.ThisMonth => Resolution.Daily,
            TimePeriod.LastMonth => Resolution.Daily,
            TimePeriod.ThisYear => Resolution.Monthly,
            TimePeriod.LastYear => Resolution.Monthly,
            _ => throw new ArgumentOutOfRangeException(nameof(timePeriod))
        };
}

internal enum Resolution { Hourly, Daily, Monthly }
