namespace HeatKeeper.Server.EnergyPriceAreas.Api;

[RequireAdminRole]
[Get("api/energy-price-areas/{Id}")]
public record GetEnergyPriceAreaDetailsQuery(long Id) : IQuery<EnergyPriceAreaDetails>;

public record EnergyPriceAreaDetails(long Id, string EIC_Code, string Name, string Description, long DisplayOrder, long VATRateId);

public class GetEnergyPriceAreaDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetEnergyPriceAreaDetailsQuery, EnergyPriceAreaDetails>
{
    public async Task<EnergyPriceAreaDetails> HandleAsync(GetEnergyPriceAreaDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<EnergyPriceAreaDetails>(sqlProvider.GetEnergyPriceAreaDetails, query)).FirstOrDefault();
}