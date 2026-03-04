using HeatKeeper.Server.Locations;

namespace HeatKeeper.Server.EnergyCosts.Api;

[RequireUserRole]
[Get("api/energy-costs")]
public record GetEnergyCostsQuery(long LocationId, long? SensorId, TimePeriod TimePeriod) : IQuery<EnergyCost>;

public record EnergyCost(Resolution Resolution, EnergyCostEntry[] TimeSeries);

public record EnergyCostEntry(DateTime Timestamp, double PowerImport, decimal CostInLocalCurrency, decimal CostInLocalCurrencyAfterSubsidy, decimal CostInLocalCurrencyWithFixedPrice);

internal record LocationEnergyCostSettings(long? SmartMeterSensorId, EnergyCalculationStrategy EnergyCalculationStrategy);

public class GetEnergyCostsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<GetEnergyCostsQuery, EnergyCost>
{
    public async Task<EnergyCost> HandleAsync(GetEnergyCostsQuery query, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (fromDateTime, toDateTime) = TimePeriodCalculator.GetDateRange(query.TimePeriod, now);
        var resolution = TimePeriodCalculator.GetResolution(query.TimePeriod);

        if (query.SensorId.HasValue)
            return new EnergyCost(resolution, await QueryBySensor(query.SensorId.Value, fromDateTime, toDateTime, resolution));

        var settings = (await dbConnection.ReadAsync<LocationEnergyCostSettings>(
            sqlProvider.GetLocationEnergyCostSettings,
            new { query.LocationId })).SingleOrDefault();

        if (settings?.EnergyCalculationStrategy == EnergyCalculationStrategy.SmartMeter)
        {
            if (settings.SmartMeterSensorId == null)
                return new EnergyCost(resolution, []);
            return new EnergyCost(resolution, await QueryBySensor(settings.SmartMeterSensorId.Value, fromDateTime, toDateTime, resolution));
        }

        return new EnergyCost(resolution, await QueryByLocation(query.LocationId, fromDateTime, toDateTime, resolution));
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

}

public enum Resolution { Hourly, Daily, Monthly }
