using CQRS.Query.Abstractions;
using HeatKeeper.Server.ExchangeRates;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;

public class ExchangeRatesTests : TestBase
{
    [Fact]
    public async Task ShouldGetExchangeRates()
    {
        var client = Factory.CreateClient();
        var handler = Factory.Services.GetRequiredService<IQueryHandler<GetExchangeRateQuery, ExchangeRateDetails>>();
        var result = await handler.HandleAsync(new GetExchangeRateQuery("EUR"));
        result.Currency.Should().Be("EUR");
        result.Rate.Should().BeGreaterThan(0);
    }
}