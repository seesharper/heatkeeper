using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Chargers;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ChargersTests : TestBase
{
    [Fact]
    public async Task ShouldGetChargers()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);
        await client.CreateCharger(TestData.Chargers.LivingRoomCharger1(outsideZoneId), testLocation.Token);
        await client.CreateCharger(TestData.Chargers.LivingRoomCharger2(outsideZoneId), testLocation.Token);

        var chargers = await client.GetChargers(outsideZoneId, testLocation.Token);

        chargers.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldAddCharger()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);

        var chargerId = await client.CreateCharger(TestData.Chargers.TestCharger(outsideZoneId), testLocation.Token);

        var charger = await client.GetChargerDetails(chargerId, testLocation.Token);

        charger.Name.Should().Be(TestData.Chargers.TestChargerName);
        charger.ZoneName.Should().Be(TestData.Zones.OutsideName);
        charger.Description.Should().Be(TestData.Chargers.TestChargerDescription);
        charger.MqttTopic.Should().Be(TestData.Chargers.TestChargerMqttTopic);
        charger.OnPayload.Should().Be(TestData.Chargers.TestChargerOnPayload);
        charger.OffPayload.Should().Be(TestData.Chargers.TestChargerOffPayload);
        charger.ChargerState.Should().Be(ChargerState.Idle);
    }

    [Fact]
    public async Task ShouldUpdateCharger()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);
        var chargerId = await client.CreateCharger(TestData.Chargers.LivingRoomCharger1(outsideZoneId), testLocation.Token);

        await client.UpdateCharger(TestData.Chargers.UpdateCharger(chargerId, outsideZoneId), testLocation.Token);

        var charger = await client.GetChargerDetails(chargerId, testLocation.Token);

        charger.Name.Should().Be(TestData.Chargers.UpdatedLivingRoomChargerName);
        charger.ZoneName.Should().Be(TestData.Zones.OutsideName);
        charger.Description.Should().Be(TestData.Chargers.UpdatedLivingRoomChargerDescription);
        charger.MqttTopic.Should().Be(TestData.Chargers.UpdatedLivingRoomChargerMqttTopic);
        charger.OnPayload.Should().Be(TestData.Chargers.UpdatedLivingRoomChargerOnPayload);
        charger.OffPayload.Should().Be(TestData.Chargers.UpdatedLivingRoomChargerOffPayload);
    }

    [Fact]
    public async Task ShouldDeleteCharger()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);
        var chargerId1 = await client.CreateCharger(TestData.Chargers.LivingRoomCharger1(outsideZoneId), testLocation.Token);
        await client.CreateCharger(TestData.Chargers.LivingRoomCharger2(outsideZoneId), testLocation.Token);

        await client.DeleteCharger(chargerId1, testLocation.Token);

        var chargers = await client.GetChargers(outsideZoneId, testLocation.Token);
        chargers.Should().HaveCount(1);
    }
}
