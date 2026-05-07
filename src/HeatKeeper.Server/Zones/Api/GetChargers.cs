namespace HeatKeeper.Server.Zones.Api;

[RequireUserRole]
[Get("/api/zones/{ZoneId}/chargers")]
public record ChargersQuery(long ZoneId) : IQuery<ChargerInfo[]>;

public class GetChargers(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ChargersQuery, ChargerInfo[]>
{
    public async Task<ChargerInfo[]> HandleAsync(ChargersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ChargerInfo>(sqlProvider.GetChargers, query)).ToArray();
}

public record ChargerInfo(long Id, string Name);
