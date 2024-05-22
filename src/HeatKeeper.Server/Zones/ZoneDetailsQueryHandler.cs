namespace HeatKeeper.Server.Zones;

public class ZoneDetailsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ZoneDetailsQuery, ZoneDetails>
{
    public async Task<ZoneDetails> HandleAsync(ZoneDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ZoneDetails>(sqlProvider.GetZoneDetails, query)).Single();
}

[RequireAdminRole]
[Get("/api/zones/{zoneId}")]
public record ZoneDetailsQuery(long ZoneId) : IQuery<ZoneDetails>;

public record ZoneDetails(long Id, string Name, string Description, long LocationId);
