namespace HeatKeeper.Server.EnergyPrices;

[RequireBackgroundRole]
public record EnergyPricesExistsQuery(long EnergyPriceAreaId, DateTime date) : IQuery<bool>;

public class EnergyPricesExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<EnergyPricesExistsQuery, bool>
{
    public async Task<bool> HandleAsync(EnergyPricesExistsQuery query, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteScalarAsync<bool>(sqlProvider.EnergyPricesExists, query);
}