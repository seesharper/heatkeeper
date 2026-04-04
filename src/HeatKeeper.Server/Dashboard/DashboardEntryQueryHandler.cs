using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Yr;

namespace HeatKeeper.Server.Dashboard;

[RequireUserRole]
[Get("api/dashboard/locations")]
public record DashboardEntryQuery : IQuery<DashboardEntry[]>;

public record DashboardLocation(long Id, string Name, string ProgramName, string ScheduleName, double? OutsideTemperature, double? InsideTemperature);

public record DashboardEntry(DashboardLocation Location, DashboardForecast Forecast, SunEvents SunEvents);

public class DashboardEntryQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, IQueryExecutor queryExecutor, TimeProvider timeProvider, IUserContext userContext) : IQueryHandler<DashboardEntryQuery, DashboardEntry[]>
{
    public async Task<DashboardEntry[]> HandleAsync(DashboardEntryQuery query, CancellationToken cancellationToken = default)
    {
        var locations = (await dbConnection.ReadAsync<DashboardLocation>(sqlProvider.GetAllDashboardLocations, new { UserId = userContext.Id })).ToArray();
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var entries = await Task.WhenAll(locations.Select(async location =>
        {
            var forecastTask = queryExecutor.ExecuteAsync(new DashboardForecastQuery(location.Id), cancellationToken);
            var sunEventsTask = queryExecutor.ExecuteAsync(new GetSunEventsQuery(location.Id, today), cancellationToken);

            await Task.WhenAll(forecastTask, sunEventsTask);
            return new DashboardEntry(location, await forecastTask, await sunEventsTask);
        }));

        return entries;
    }
}
