namespace HeatKeeper.Server.EnergyCosts;

public record SensorEnergyContext(long SensorId, long? EnergyPriceAreaId, double FixedEnergyPrice, bool UseFixedEnergyPrice);

[RequireReporterRole]
public record GetSensorEnergyContextQuery(string ExternalSensorId) : IQuery<SensorEnergyContext[]>;

public class GetSensorEnergyContextQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetSensorEnergyContextQuery, SensorEnergyContext[]>
{
    public async Task<SensorEnergyContext[]> HandleAsync(GetSensorEnergyContextQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SensorEnergyContext>(sqlProvider.GetSensorEnergyContext, query)).ToArray();
}
