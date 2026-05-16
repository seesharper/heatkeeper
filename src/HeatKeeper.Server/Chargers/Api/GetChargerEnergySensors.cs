namespace HeatKeeper.Server.Chargers.Api;

[RequireUserRole]
[Get("api/chargers/{chargerId}/energy-sensors")]
public record ChargerEnergySensorsQuery(long ChargerId) : IQuery<ChargerEnergySensorInfo[]>;

public record ChargerEnergySensorInfo(long Id, string Name, string ExternalId);

public class GetChargerEnergySensors(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ChargerEnergySensorsQuery, ChargerEnergySensorInfo[]>
{
    public async Task<ChargerEnergySensorInfo[]> HandleAsync(ChargerEnergySensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ChargerEnergySensorInfo>(sqlProvider.GetChargerEnergySensors, query)).ToArray();
}
