namespace HeatKeeper.Server.Sensors.Api;

[RequireBackgroundRole]
[Get("/api/sensors/deadSensors")]
public record DeadSensorsQuery() : IQuery<DeadSensor[]>;

public record DeadSensor(long Id, string Name, string ExternalId, string Zone, string Location, DateTime LastSeen);

public class GetDeadSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<DeadSensorsQuery, DeadSensor[]>
{
    public async Task<DeadSensor[]> HandleAsync(DeadSensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<DeadSensor>(sqlProvider.GetDeadSensors, new { Now = timeProvider.GetUtcNow().DateTime })).ToArray();
}