using System;
using CQRS.AspNet.Testing;
using CsvHelper;
using HeatKeeper.Server.EnergyPrices;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;


public class EntsoeTests : TestBase
{
    [Fact]
    public async Task ShouldGetrMarketDocument()
    {
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "f7e3c0e2-9e71-4225-97d6-91b69be1acd4");
        var client = Factory.Services.GetRequiredService<EntsoeClient>();
        var marketDocument = await client.GetMarketDocument(DateTime.UtcNow.Date, "10YNO-2--------T");
        marketDocument.Should().NotBeNull();
    }
}