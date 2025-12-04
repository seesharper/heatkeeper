using System;
using CQRS.AspNet.Testing;
using CsvHelper;
using HeatKeeper.Server.EnergyPrices;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;


public class EntsoeTests : TestBase
{
    [Fact(Skip = "Platform temporarily lacks internet access")]
    public async Task ShouldGetrMarketDocument()
    {
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "25dabe3a-9fe1-4074-8cca-b2b3100b26a8");
        var client = Factory.Services.GetRequiredService<EntsoeClient>();
        var marketDocument = await client.GetMarketDocument(DateTime.UtcNow.Date, "10YNO-2--------T");
        marketDocument.Should().NotBeNull();
    }
}