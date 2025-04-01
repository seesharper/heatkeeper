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
    [Fact]
    public async Task ShouldImportElectricalPricesUsingJanitor()
    {
        Factory.UseFakeTimeProvider(TestData.Clock.Today);
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "d8e25188-7571-47c5-90b0-ec7b1b150e06");
        var janitor = Factory.Services.GetService<IJanitor>();
        await janitor.Run("ImportEnergyPrices");
    }

    [Fact]
    public async Task ShouldImportEnergyPrices()
    {
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "d8e25188-7571-47c5-90b0-ec7b1b150e06");
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        var dateToImport = new DateTime(2024, 12, 14);
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(dateToImport), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(dateToImport.ToString("yyyy-MM-dd"), testLocation.PriceAreaId, testLocation.Token);
        energyPrices.Length.Should().Be(24);
    }

    [Fact]
    public async Task ShouldNotImportEnergyPricesTwice()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(DateTime.UtcNow), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(DateTime.UtcNow.ToString("yyyy-MM-dd"), testLocation.PriceAreaId, testLocation.Token);
        energyPrices.Length.Should().Be(24);
        energyPrices.All(x => x.Price > 0).Should().BeTrue();
        energyPrices.All(x => x.PriceAfterSubsidy > 0).Should().BeTrue();
    }
}