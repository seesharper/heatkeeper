namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Get("/api/sensors")]
public record UnassignedSensorsQuery() : IQuery<UnassignedSensorInfo[]>;

public class GetUnassignedSensors(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<UnassignedSensorsQuery, UnassignedSensorInfo[]>
{
    public async Task<UnassignedSensorInfo[]> HandleAsync(UnassignedSensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UnassignedSensorInfo>(sqlProvider.GetUnassignedSensors)).ToArray();
}

public record UnassignedSensorInfo(long Id, string Name, string ExternalId, DateTime LastSeen);