namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Get("api/locations")]
public record GetLocationsQuery : IQuery<LocationInfo[]>;

public class GetLocations(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetLocationsQuery, LocationInfo[]>
{
    public async Task<LocationInfo[]> HandleAsync(GetLocationsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LocationInfo>(sqlProvider.GetAllLocations, query)).ToArray();
}

public record LocationInfo(long Id, string Name);
