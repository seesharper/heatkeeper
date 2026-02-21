namespace HeatKeeper.Server.EnergyCosts;

public record HourlyEnergyPrice(decimal PriceInLocalCurrency, decimal PriceInLocalCurrencyAfterSubsidy);

[RequireReporterRole]
public record GetEnergyPriceForHourQuery(long EnergyPriceAreaId, DateTime Hour) : IQuery<HourlyEnergyPrice[]>;

public class GetEnergyPriceForHourQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetEnergyPriceForHourQuery, HourlyEnergyPrice[]>
{
    public async Task<HourlyEnergyPrice[]> HandleAsync(GetEnergyPriceForHourQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<HourlyEnergyPrice>(sqlProvider.GetEnergyPriceForHour, query)).ToArray();
}
