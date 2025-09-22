using System.Data;
using DbReader;

namespace HeatKeeper.Server.Locations.Api;

/// <summary>
/// Represents location coordinates for external services like outdoor lighting.
/// </summary>
public record LocationCoordinates(long Id, string Name, double? Latitude, double? Longitude);

/// <summary>
/// Query to get all location coordinates for background services.
/// </summary>
[RequireBackgroundRole]
public record GetLocationCoordinatesQuery() : IQuery<LocationCoordinates[]>;

public class GetLocationCoordinates(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetLocationCoordinatesQuery, LocationCoordinates[]>
{
    public async Task<LocationCoordinates[]> HandleAsync(GetLocationCoordinatesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LocationCoordinates>(sqlProvider.GetLocationCoordinates)).ToArray();
}