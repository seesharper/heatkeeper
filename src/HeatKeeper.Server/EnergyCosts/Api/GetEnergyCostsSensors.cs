namespace HeatKeeper.Server.EnergyCosts.Api;

[RequireUserRole]
[Get("api/energycosts/sensors")]
public record GetEnergyCostsSensorsQuery(long LocationId) : IQuery<EnergyCostSensorInfo[]>;

public record EnergyCostSensorInfo(long Id, string Name);

public class GetEnergyCostsSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    : IQueryHandler<GetEnergyCostsSensorsQuery, EnergyCostSensorInfo[]>
{
    public async Task<EnergyCostSensorInfo[]> HandleAsync(GetEnergyCostsSensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<EnergyCostSensorInfo>(sqlProvider.GetEnergyCostsSensors, new { query.LocationId })).ToArray();
}
