namespace HeatKeeper.Server.EnergyPrices.Api;

[RequireUserRole]
[Get("api/energy-prices")]
public record GetEnergyPricesQuery(long EnergyPriceAreaId, string Date) : IQuery<EnergyPrice[]>;

public record EnergyPrice(DateTime Date, decimal Price, decimal PriceAfterSubsidy);

public class EnergyPricesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetEnergyPricesQuery, EnergyPrice[]>
{
    public async Task<EnergyPrice[]> HandleAsync(GetEnergyPricesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<EnergyPrice>(sqlProvider.GetEnergyPrices, query)).ToArray();
}