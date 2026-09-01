using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Chargers;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
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
        charger.EnergySensorId.Should().BeNull();
        charger.EnergyThreshold.Should().Be(0);
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

    [Fact]
    public async Task ShouldGetAvailableEnergySensors()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);

        var unassignedSensors = await client.GetUnassignedSensors(testLocation.Token);
        var outsideSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

        var chargerAId = await client.CreateCharger(
            TestData.Chargers.LivingRoomCharger1(outsideZoneId) with { EnergySensorId = outsideSensor.Id },
            testLocation.Token);
        var chargerBId = await client.CreateCharger(TestData.Chargers.LivingRoomCharger2(outsideZoneId), testLocation.Token);

        var availableForB = await client.GetChargerEnergySensors(chargerBId, testLocation.Token);
        availableForB.Should().NotContain(s => s.Id == outsideSensor.Id);

        var availableForA = await client.GetChargerEnergySensors(chargerAId, testLocation.Token);
        availableForA.Should().Contain(s => s.Id == outsideSensor.Id);
    }

    [Fact]
    public async Task ShouldSetChargerStateToActiveWhenEnergyAboveThreshold()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);

        var unassignedSensors = await client.GetUnassignedSensors(testLocation.Token);
        var outsideSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

        var chargerId = await client.CreateCharger(
            TestData.Chargers.LivingRoomCharger1(outsideZoneId) with { EnergySensorId = outsideSensor.Id, EnergyThreshold = 100 },
            testLocation.Token);

        await client.CreateMeasurements(
            [new MeasurementCommand(TestData.Sensors.OutsideSensor, MeasurementType.Temperature, RetentionPolicy.Day, 200, DateTime.UtcNow)],
            testLocation.Token);

        var charger = await client.GetChargerDetails(chargerId, testLocation.Token);
        charger.ChargerState.Should().Be(ChargerState.Active);
    }

    [Fact]
    public async Task ShouldSetChargerStateToIdleWhenEnergyBelowThreshold()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var outsideZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.Outside, testLocation.Token);

        var unassignedSensors = await client.GetUnassignedSensors(testLocation.Token);
        var outsideSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

        var chargerId = await client.CreateCharger(
            TestData.Chargers.LivingRoomCharger1(outsideZoneId) with { EnergySensorId = outsideSensor.Id, EnergyThreshold = 100 },
            testLocation.Token);

        // First push state to Active
        await client.CreateMeasurements(
            [new MeasurementCommand(TestData.Sensors.OutsideSensor, MeasurementType.Temperature, RetentionPolicy.Day, 200, DateTime.UtcNow)],
            testLocation.Token);

        // Then drop below threshold
        await client.CreateMeasurements(
            [new MeasurementCommand(TestData.Sensors.OutsideSensor, MeasurementType.Temperature, RetentionPolicy.Day, 50, DateTime.UtcNow)],
            testLocation.Token);

        var charger = await client.GetChargerDetails(chargerId, testLocation.Token);
        charger.ChargerState.Should().Be(ChargerState.Idle);
    }
}
