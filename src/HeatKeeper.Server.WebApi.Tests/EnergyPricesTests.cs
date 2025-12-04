using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using HeatKeeper.Server.EnergyPrices.Api;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class EnergyPricesTests : TestBase
{
    [Fact(Skip = "Platform temporarily lacks internet access")]
    public async Task ShouldImportElectricalPricesUsingJanitor()
    {
        Factory.UseFakeTimeProvider(TestData.Clock.Today);
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "25dabe3a-9fe1-4074-8cca-b2b3100b26a8");
        var janitor = Factory.Services.GetService<IJanitor>();
        await janitor.Run("ImportEnergyPrices");
    }

    [Fact(Skip = "Platform temporarily lacks internet access")]
    public async Task ShouldImportEnergyPrices()
    {
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "25dabe3a-9fe1-4074-8cca-b2b3100b26a8");
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        var dateToImport = new DateTime(2025, 12, 14);
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(dateToImport), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(dateToImport.ToString("yyyy-MM-dd"), testLocation.PriceAreaId, testLocation.Token);
        energyPrices.Length.Should().BeGreaterThanOrEqualTo(24);
    }

    [Fact(Skip = "Platform temporarily lacks internet access")]
    public async Task ShouldNotImportEnergyPricesTwice()
    {
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "25dabe3a-9fe1-4074-8cca-b2b3100b26a8");
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(DateTime.UtcNow), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(DateTime.UtcNow.ToString("yyyy-MM-dd"), testLocation.PriceAreaId, testLocation.Token);
        energyPrices.All(x => x.Price > 0).Should().BeTrue();
        energyPrices.All(x => x.PriceAfterSubsidy > 0).Should().BeTrue();
    }
}