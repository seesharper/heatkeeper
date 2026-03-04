namespace HeatKeeper.Server.ZoneTemperatures;

public record ZoneTemperature(long ZoneId, double Temperature, DateTime Hour, DateTime LastUpdate);

[RequireReporterRole]
public record GetZoneTemperaturesQuery(long ZoneId) : IQuery<ZoneTemperature[]>;

public class GetZoneTemperaturesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    : IQueryHandler<GetZoneTemperaturesQuery, ZoneTemperature[]>
{
    public async Task<ZoneTemperature[]> HandleAsync(GetZoneTemperaturesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ZoneTemperature>(sqlProvider.GetZoneTemperatures, query)).ToArray();
}
