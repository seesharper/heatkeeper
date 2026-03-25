using System.Data;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.EnergyCosts.Api;
using HeatKeeper.Server.ZoneTemperatures;

namespace HeatKeeper.Server.Zones.Api;

[RequireUserRole]
[Get("/api/zones/{ZoneId}/insights")]
public record GetZoneInsightsQuery(long ZoneId, TimePeriod TimePeriod = TimePeriod.Today) : IQuery<ZoneInsightsResult>;

public class GetZoneInsightsQueryHandler(IQueryExecutor queryExecutor) : IQueryHandler<GetZoneInsightsQuery, ZoneInsightsResult>
{
    public async Task<ZoneInsightsResult> HandleAsync(GetZoneInsightsQuery query, CancellationToken cancellationToken = default)
    {
        var temperatures = await queryExecutor.ExecuteAsync(new ZoneTemperaturesPerZoneQuery(query.ZoneId, query.TimePeriod), cancellationToken);
        var energyCosts = await queryExecutor.ExecuteAsync(new EnergyCostsPerZoneQuery(query.ZoneId, query.TimePeriod), cancellationToken);
        var setPoints = await queryExecutor.ExecuteAsync(new SetPointsByZoneForActiveProgramQuery(query.ZoneId), cancellationToken);
        return new ZoneInsightsResult(temperatures, energyCosts, setPoints);
    }
}

public record ZoneInsightsResult(ZoneTemperatureResult Temperatures, EnergyCost EnergyCosts, ZoneSetPoint[] SetPoints);

public record ZoneSetPoint(long Id, string ScheduleName, double Value, double Hysteresis);

[RequireUserRole]
public record SetPointsByZoneForActiveProgramQuery(long ZoneId) : IQuery<ZoneSetPoint[]>;

public class GetSetPointsByZoneForActiveProgramQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SetPointsByZoneForActiveProgramQuery, ZoneSetPoint[]>
{
    public async Task<ZoneSetPoint[]> HandleAsync(SetPointsByZoneForActiveProgramQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ZoneSetPoint>(sqlProvider.GetSetPointsByZoneForActiveProgram, query)).ToArray();
}
