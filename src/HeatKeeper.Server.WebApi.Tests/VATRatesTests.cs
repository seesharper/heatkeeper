using HeatKeeper.Server.VATRates;

namespace HeatKeeper.Server.WebApi.Tests;

public class VATRatesTests : TestBase
{
    [Fact]
    public async Task ShouldCreateVATRate()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateVATRate(new PostVATRateCommand("VATRate", 25), testLocation.Token);
        id.Should().BeGreaterThan(0);                 
    }

    [Fact]
    public async Task ShouldUpdateVATRate()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateVATRate(new VATRates.PostVATRateCommand("VATRate", 25), testLocation.Token);
        await client.UpdateVATRate(new VATRates.PatchVATRateCommand(id, "UpdatesVATRate", 30), testLocation.Token);
        var vatRate = await client.GetVATRateDetails(id, testLocation.Token);
        vatRate.Name.Should().Be("UpdatesVATRate");
        vatRate.Rate.Should().Be(30);
    }

    [Fact]
    public async Task ShouldDeleteVATRate()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateVATRate(new VATRates.PostVATRateCommand("VATRate", 25), testLocation.Token);
        await client.DeleteVATRate(id, testLocation.Token);
        var vatRates = await client.GetVATRates(testLocation.Token);
        vatRates.Should().NotContain(x => x.Id == id);
    }
}