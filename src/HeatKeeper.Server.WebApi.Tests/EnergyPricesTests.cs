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
        Factory.WithConfiguration("ENTSOE_SECURITY_TOKEN", "f7e3c0e2-9e71-4225-97d6-91b69be1acd4");
        var janitor = Factory.Services.GetService<IJanitor>();
        await janitor.Run("ImportEnergyPrices");
    }

    [Fact]
    public async Task ShouldImportEnergyPrices()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(DateTime.UtcNow), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(DateTime.UtcNow.ToString("yyyy-MM-dd"), testLocation.PriceAreaId, testLocation.Token);
        energyPrices.Length.Should().Be(24);
    }

    [Fact]
    public async Task ShouldNotImportEnergyPricesTwice()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = Factory.CreateClient();
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(DateTime.UtcNow), testLocation.Token);
        await client.ImportEnergyPrices(new ImportEnergyPricesCommand(DateTime.UtcNow), testLocation.Token);
        var energyPrices = await client.GetEnergyPrices(DateTime.UtcNow.ToString("yyyy-MM-dd"),testLocation.PriceAreaId, testLocation.Token);
        energyPrices.Length.Should().Be(24);
    }
}