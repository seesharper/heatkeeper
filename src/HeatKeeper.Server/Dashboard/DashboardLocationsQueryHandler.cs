namespace HeatKeeper.Server.Dashboard;

public class DashboardLocationsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<DashboardLocationsQuery, DashboardLocation[]>
{
    public async Task<DashboardLocation[]> HandleAsync(DashboardLocationsQuery query, CancellationToken cancellationToken = default)
    {
        return (await dbConnection.ReadAsync<DashboardLocation>(sqlProvider.GetAllDashboardLocations)).ToArray();
    }
}

[RequireUserRole]
[Get("api/dashboard/locations")]
public record DashboardLocationsQuery : IQuery<DashboardLocation[]>;

public record DashboardLocation(long Id, string Name, double? OutsideTemperature, double? OutsideHumidity, double? InsideTemperature, double? InsideHumidity);
