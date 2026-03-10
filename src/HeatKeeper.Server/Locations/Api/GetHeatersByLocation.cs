using HeatKeeper.Server.Heaters;

namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Get("api/locations/{locationId}/heaters")]
public record HeatersByLocationQuery(long LocationId) : IQuery<LocationHeaterInfo[]>;

public record LocationHeaterInfo(long Id, string Name, HeaterState HeaterState);

public class GetHeatersByLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<HeatersByLocationQuery, LocationHeaterInfo[]>
{
    public async Task<LocationHeaterInfo[]> HandleAsync(HeatersByLocationQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LocationHeaterInfo>(sqlProvider.GetHeatersByLocation, query)).ToArray();
}
