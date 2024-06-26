namespace HeatKeeper.Server.Sensors;

[RequireAdminRole]
public record UnassignedSensorsQuery() : IQuery<UnassignedSensorInfo[]>;

public record UnassignedSensorInfo(long Id, string Name, string ExternalId, DateTime LastSeen);

public class GetUnassignedSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<UnassignedSensorsQuery, UnassignedSensorInfo[]>
{
    public async Task<UnassignedSensorInfo[]> HandleAsync(UnassignedSensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UnassignedSensorInfo>(sqlProvider.GetUnassignedSensors)).ToArray();
}