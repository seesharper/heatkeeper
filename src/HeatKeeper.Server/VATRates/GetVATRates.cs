namespace HeatKeeper.Server.VATRates;

[RequireAdminRole]
[Get("api/vat-rates")]
public record GetVATRatesQuery() : IQuery<VATRateInfo[]>;

public record VATRateInfo(long Id, string Name, decimal Rate);

public class GetVATRates(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetVATRatesQuery, VATRateInfo[]>
{
    public async Task<VATRateInfo[]> HandleAsync(GetVATRatesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<VATRateInfo>(sqlProvider.GetVATRates)).ToArray();
}