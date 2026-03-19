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
        return new ZoneInsightsResult(temperatures, energyCosts);
    }
}

public record ZoneInsightsResult(ZoneTemperatureResult Temperatures, EnergyCost EnergyCosts);
