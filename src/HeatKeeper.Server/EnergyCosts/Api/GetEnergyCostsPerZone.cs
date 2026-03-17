namespace HeatKeeper.Server.EnergyCosts.Api;

[RequireBackgroundRole]
public record EnergyCostsPerZoneQuery(long ZoneId, TimePeriod TimePeriod) : IQuery<EnergyCost>;

public class GetEnergyCostsPerZoneQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, TimeProvider timeProvider) : IQueryHandler<EnergyCostsPerZoneQuery, EnergyCost>
{
    public async Task<EnergyCost> HandleAsync(EnergyCostsPerZoneQuery query, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (fromDateTime, toDateTime) = TimePeriodCalculator.GetDateRange(query.TimePeriod, now);
        var resolution = TimePeriodCalculator.GetResolution(query.TimePeriod);

        var sql = resolution switch
        {
            Resolution.Hourly => sqlProvider.GetEnergyCostsByHourForZone,
            Resolution.Daily => sqlProvider.GetEnergyCostsByDayForZone,
            Resolution.Monthly => sqlProvider.GetEnergyCostsByMonthForZone,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution))
        };

        var entries = (await dbConnection.ReadAsync<EnergyCostEntry>(sql, new { query.ZoneId, FromDateTime = fromDateTime, ToDateTime = toDateTime })).ToArray();
        return new EnergyCost(resolution, entries);
    }
}
