using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.EnergyCosts.Api;

namespace HeatKeeper.Server.ZoneTemperatures;

[RequireBackgroundRole]
public record ZoneTemperaturesPerZoneQuery(long ZoneId, TimePeriod TimePeriod) : IQuery<ZoneTemperatureResult>;

public record ZoneTemperatureResult(Resolution Resolution, ZoneTemperatureEntry[] TimeSeries);

public record ZoneTemperatureEntry(DateTime Timestamp, double Temperature);

public class GetZoneTemperaturesPerZoneQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<ZoneTemperaturesPerZoneQuery, ZoneTemperatureResult>
{
    public async Task<ZoneTemperatureResult> HandleAsync(ZoneTemperaturesPerZoneQuery query, CancellationToken cancellationToken = default)
    {
        var timeZoneId = (await dbConnection.ReadAsync<string?>(sqlProvider.GetTimeZoneByZoneId, new { query.ZoneId })).SingleOrDefault();
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (fromDateTime, toDateTime) = TimePeriodCalculator.GetDateRange(query.TimePeriod, now, timeZoneId);
        var resolution = TimePeriodCalculator.GetResolution(query.TimePeriod);

        var sql = resolution switch
        {
            Resolution.Hourly => sqlProvider.GetZoneTemperaturesByHourForZone,
            Resolution.Daily => sqlProvider.GetZoneTemperaturesByDayForZone,
            Resolution.Monthly => sqlProvider.GetZoneTemperaturesByMonthForZone,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution))
        };

        var entries = (await dbConnection.ReadAsync<ZoneTemperatureEntry>(sql, new { query.ZoneId, FromDateTime = fromDateTime, ToDateTime = toDateTime })).ToArray();
        return new ZoneTemperatureResult(resolution, entries);
    }
}
