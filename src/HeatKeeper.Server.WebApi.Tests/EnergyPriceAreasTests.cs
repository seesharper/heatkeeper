
using HeatKeeper.Server.EnergyPriceAreas.Api;

namespace HeatKeeper.Server.WebApi.Tests;


public class EnergyPriceAreasTests : TestBase
{
    [Fact]
    public async Task ShouldPostEnergyPriceArea()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateEnergyPriceArea(new PostEnergyPriceAreaCommand("EIC_Codse", "Name", "Description", 1, testLocation.VATRateId), testLocation.Token);
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldPatchEnergyPriceArea()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateEnergyPriceArea(new PostEnergyPriceAreaCommand("EIC_Code", "Name", "Description", 1, testLocation.VATRateId), testLocation.Token);
        await client.UpdateEnergyPriceArea(new PatchEnergyPriceAreaCommand(id, "UpdatedEIC_Code", "UpdatedName", "UpdatedDescription", 2, testLocation.VATRateId), testLocation.Token);
        var energyPriceArea = await client.GetEnergyPriceAreaDetails(id, testLocation.Token);
        energyPriceArea.EIC_Code.Should().Be("UpdatedEIC_Code");
        energyPriceArea.Name.Should().Be("UpdatedName");
        energyPriceArea.Description.Should().Be("UpdatedDescription");
        energyPriceArea.DisplayOrder.Should().Be(2);
    }

    [Fact]
    public async Task ShouldDeleteEnergyPriceArea()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var id = await client.CreateEnergyPriceArea(new PostEnergyPriceAreaCommand("EIC_Code", "Name", "Description", 1, testLocation.VATRateId), testLocation.Token);
        await client.DeleteEnergyPriceArea(id, testLocation.Token);
        var energyPriceAreas = await client.GetEnergyPriceAreas(testLocation.Token);
        energyPriceAreas.Should().NotContain(x => x.Id == id);
    }
}