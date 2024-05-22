using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class HeatersTests : TestBase
{
    [Fact]
    public async Task ShouldGetHeaters()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heaters = await client.GetHeaters(testLocation.LivingRoomZoneId, testLocation.Token);
        heaters.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldAddHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heaterId = await client.CreateHeater(TestData.Heaters.TestHeater(testLocation.LivingRoomZoneId), testLocation.Token);

        var heater = await client.GetHeatersDetails(heaterId, testLocation.Token);

        heater.Name.Should().Be(TestData.Heaters.TestHeaterName);
        heater.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        heater.Description.Should().Be(TestData.Heaters.TestHeaterDescription);
        heater.MqttTopic.Should().Be(TestData.Heaters.TestHeaterMqttTopic);
        heater.OnPayload.Should().Be(TestData.Heaters.TestHeaterOnPayload);
        heater.OffPayload.Should().Be(TestData.Heaters.TestHeaterOffPayload);
    }

    [Fact]
    public async Task ShouldUpdateHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.UpdateHeater(TestData.Heaters.UpdateHeater(testLocation.LivingRoomHeaterId1), testLocation.Token);

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);


        heater.Name.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterName);
        heater.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        heater.Description.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterDescription);
        heater.MqttTopic.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterMqttTopic);
        heater.OnPayload.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterOnPayload);
        heater.OffPayload.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterOffPayload);
    }

    [Fact]
    public async Task ShouldDeleteHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.DeleteHeater(testLocation.LivingRoomHeaterId1, testLocation.Token);

        var heaters = await client.GetHeaters(testLocation.LivingRoomZoneId, testLocation.Token);
        heaters.Should().HaveCount(1);
    }
}