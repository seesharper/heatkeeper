namespace HeatKeeper.Server.Zones.Api;

[RequireUserRole]
[Get("/api/zones/{ZoneId}/heaters")]
public record HeatersQuery(long ZoneId) : IQuery<HeaterInfo[]>;

public class GetHeaters(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<HeatersQuery, HeaterInfo[]>
{
    public async Task<HeaterInfo[]> HandleAsync(HeatersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<HeaterInfo>(sqlProvider.GetHeaters, query)).ToArray();
}

public record HeaterInfo(long Id, string Name);