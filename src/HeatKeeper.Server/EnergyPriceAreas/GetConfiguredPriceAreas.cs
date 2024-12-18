namespace HeatKeeper.Server.EnergyPriceAreas;

[RequireBackgroundRole]
public record GetConfiguredPriceAreasQuery() : IQuery<ConfiguredPriceArea[]>;

public record ConfiguredPriceArea(long EnergyPriceAreaId, string EIC_Code, decimal VATRate);

public class GetConfiguredPriceAreasQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetConfiguredPriceAreasQuery, ConfiguredPriceArea[]>
{
    public async Task<ConfiguredPriceArea[]> HandleAsync(GetConfiguredPriceAreasQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ConfiguredPriceArea>(sqlProvider.GetConfiguredPriceAreas, query)).ToArray();
}

