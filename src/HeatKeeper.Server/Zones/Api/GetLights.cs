namespace HeatKeeper.Server.Zones.Api;

[RequireUserRole]
[Get("/api/zones/{ZoneId}/lights")]
public record LightsQuery(long ZoneId) : IQuery<LightInfo[]>;

public class GetLights(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<LightsQuery, LightInfo[]>
{
    public async Task<LightInfo[]> HandleAsync(LightsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LightInfo>(sqlProvider.GetLights, query)).ToArray();
}

public record LightInfo(long Id, string Name);
