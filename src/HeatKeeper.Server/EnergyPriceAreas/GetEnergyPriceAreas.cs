namespace HeatKeeper.Server.EnergyPriceAreas;

[RequireAdminRole]
[Get("api/energy-price-areas")]
public record GetEnergyPriceAreasQuery() : IQuery<EnergyPriceAreaInfo[]>;

public record EnergyPriceAreaInfo(long Id, string Name);

public class GetEnergyPriceAreas(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetEnergyPriceAreasQuery, EnergyPriceAreaInfo[]>
{
    public async Task<EnergyPriceAreaInfo[]> HandleAsync(GetEnergyPriceAreasQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<EnergyPriceAreaInfo>(sqlProvider.GetEnergyPriceAreas)).ToArray();
}