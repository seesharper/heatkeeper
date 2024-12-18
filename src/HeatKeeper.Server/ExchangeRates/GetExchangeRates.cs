
namespace HeatKeeper.Server.ExchangeRates;

[RequireBackgroundRole]
public record GetExchangeRateQuery(string currency) : IQuery<ExchangeRateDetails>;

public record ExchangeRateDetails(string Currency, decimal Rate);

public class GetExchangeQueryHandler(NorwegianBankClient norwegianBankClient) : IQueryHandler<GetExchangeRateQuery, ExchangeRateDetails>
{
    public async Task<ExchangeRateDetails> HandleAsync(GetExchangeRateQuery query, CancellationToken cancellationToken = default)
    {
        var rate = await norwegianBankClient.GetExchangeRate(query.currency);
        return new ExchangeRateDetails(query.currency, rate);
    }
}