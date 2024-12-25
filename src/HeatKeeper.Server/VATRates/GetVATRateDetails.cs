namespace HeatKeeper.Server.VATRates;

[RequireAdminRole]
[Get("api/vat-rates/{Id}")]
public record GetVATRateDetailsQuery(long Id) : IQuery<VATRateDetails>;

public record VATRateDetails(long Id, string Name, decimal Rate);

public class GetVATRateDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetVATRateDetailsQuery, VATRateDetails>
{
    public async Task<VATRateDetails> HandleAsync(GetVATRateDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<VATRateDetails>(sqlProvider.GetVATRateDetails, query)).Single();
}