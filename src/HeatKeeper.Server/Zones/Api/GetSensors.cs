namespace HeatKeeper.Server.Zones.Api;

[RequireAdminRole]
[Get("/api/zones/{ZoneId}/sensors")]
public record SensorsQuery(long ZoneId) : IQuery<SensorInfo[]>;

public class GetSensors(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SensorsQuery, SensorInfo[]>
{
    public async Task<SensorInfo[]> HandleAsync(SensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SensorInfo>(sqlProvider.GetSensorsByZone, query)).ToArray();
}

public record SensorInfo(long Id, string Name);
