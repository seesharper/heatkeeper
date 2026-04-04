using HeatKeeper.Server.Yr;

namespace HeatKeeper.Server.Dashboard;

[RequireBackgroundRole]
public record DashboardForecastQuery(long LocationId) : IQuery<DashboardForecast>;

public record DashboardForecast(
    DashboardForecastPeriod Instant,
    DashboardForecastPeriod FirstPeriod,
    DashboardForecastPeriod SecondPeriod,
    DashboardForecastPeriod ThirdPeriod,
    DashboardForecastPeriod FourthPeriod);

public record DashboardForecastPeriod(DateTimeOffset From, DateTimeOffset To, string SymbolCode, double? Temperature);

public class DashboardForecastQueryHandler(IQueryExecutor queryExecutor, IDbConnection dbConnection, TimeProvider timeProvider) : IQueryHandler<DashboardForecastQuery, DashboardForecast>
{
    public async Task<DashboardForecast> HandleAsync(DashboardForecastQuery query, CancellationToken cancellationToken = default)
    {
        var location = (await dbConnection.ReadAsync<LocationCoordinatesWithTimeZone>(
            "SELECT Latitude, Longitude, TimeZone FROM Locations WHERE Id = @LocationId", query)).Single();

        var locationForecast = await queryExecutor.ExecuteAsync(
            new GetLocationForecastQuery((double)location.Latitude, (double)location.Longitude),
            cancellationToken);

        var timeSeries = locationForecast.Properties.TimeSeries;
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(location.TimeZone ?? "UTC");
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timezone);

        // Instant: most recent entry at or before now, symbol from next_6_hours
        var instantEntry = timeSeries
            .Where(ts => ts.Time.UtcDateTime <= nowUtc && ts.Data.Next6Hours is not null)
            .MaxBy(ts => ts.Time);

        var instant = instantEntry is not null
            ? new DashboardForecastPeriod(
                From: instantEntry.Time,
                To: instantEntry.Time,
                SymbolCode: instantEntry.Data.Next6Hours.Summary.SymbolCode,
                Temperature: instantEntry.Data.Instant.Details.AirTemperature)
            : null;

        // Determine the current 6-hour block in local time, then build 4 consecutive periods
        var blockStartHour = (localNow.Hour / 6) * 6;
        var localBlockStart = new DateTime(localNow.Year, localNow.Month, localNow.Day, blockStartHour, 0, 0);

        var periods = Enumerable.Range(0, 4)
            .Select(i =>
            {
                var localFrom = localBlockStart.AddHours(i * 6);
                var utcFrom = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(localFrom, timezone));
                var utcTo = utcFrom.AddHours(6);
                var utcMid = utcFrom.AddHours(3);
                return GetPeriod(timeSeries, utcFrom, utcTo, utcMid);
            })
            .ToArray();

        return new DashboardForecast(instant, periods[0], periods[1], periods[2], periods[3]);
    }

    private static DashboardForecastPeriod GetPeriod(
        IEnumerable<HeatKeeper.Server.Yr.TimeSeriesItem> timeSeries,
        DateTimeOffset from,
        DateTimeOffset to,
        DateTimeOffset mid)
    {
        var list = timeSeries.ToList();

        var startEntry = list.FirstOrDefault(ts =>
            ts.Time.UtcDateTime == from.UtcDateTime &&
            ts.Data.Next6Hours is not null);

        if (startEntry is null)
            return null;

        var midEntry = list.FirstOrDefault(ts => ts.Time.UtcDateTime == mid.UtcDateTime);

        return new DashboardForecastPeriod(
            From: from,
            To: to,
            SymbolCode: startEntry.Data.Next6Hours.Summary.SymbolCode,
            Temperature: midEntry?.Data.Instant.Details.AirTemperature);
    }
}

internal record LocationCoordinatesWithTimeZone(double? Latitude, double? Longitude, string TimeZone);
