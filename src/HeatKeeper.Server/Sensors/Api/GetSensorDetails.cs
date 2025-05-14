namespace HeatKeeper.Server.Sensors.Api;

[RequireAdminRole]
[Get("/api/sensors/{sensorId}")]
public record SensorDetailsQuery(long SensorId) : IQuery<SensorDetails>;

public record SensorDetails(long Id, string Name, string Description, string ExternalId, long MinutesBeforeConsideredDead, DateTime LastSeen, string ZoneName);

public class GetSensorDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SensorDetailsQuery, SensorDetails>
{
    public async Task<SensorDetails> HandleAsync(SensorDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SensorDetails>(sqlProvider.GetSensorDetails, query)).Single();
}