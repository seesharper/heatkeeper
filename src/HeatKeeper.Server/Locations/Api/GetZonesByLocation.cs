using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Get("api/locations/{locationId}/zones")]
public record ZonesByLocationQuery(long LocationId) : IQuery<ZoneInfo[]>;

public class GetZonesByLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ZonesByLocationQuery, ZoneInfo[]>
{
    public async Task<ZoneInfo[]> HandleAsync(ZonesByLocationQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ZoneInfo>(sqlProvider.ZonesByLocation, query)).ToArray();
}



