using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using FluentAssertions;
using HeatKeeper.Server.Insights.Zones;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Zones;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ZonesTests : TestBase
{
    public ZonesTests()
    {
    }

    [Fact]
    public async Task ShouldGetZonesForLocation()
    {
        var testLocation = await Factory.CreateTestLocation();
        var zones = await testLocation.HttpClient.GetZones(testLocation.LocationId, testLocation.Token);
        zones.Length.Should().Be(2);
    }

    [Fact]
    public async Task ShouldGetUnassignedSensorsForZone()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

        var sensors = await client.GetUnassignedSensors(token);

        sensors.Length.Should().Be(2);
    }

    [Fact]
    public async Task ShouldAssignZoneToSensor()
    {
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        await client.CreateMeasurements(TestData.KitchenTemperatureMeasurements, testApplication.Token);

        var unassignedSensors = await client.GetUnassignedSensors(testApplication.Token);
        unassignedSensors.Length.Should().Be(1);

        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(unassignedSensors[0].Id, testApplication.KitchenZoneId), testApplication.Token);

        var kitchenSensors = await client.GetSensors(testApplication.KitchenZoneId, testApplication.Token);
        kitchenSensors.Length.Should().Be(1);
    }

    [Fact]
    public async Task ShouldRemoveZoneFromSensor()
    {
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        await client.RemovedZoneFromSensor(new RemoveZoneFromSensorCommand(testApplication.LivingRoomSensorId), testApplication.Token);

        var unassignedSensors = await client.GetUnassignedSensors(testApplication.Token);
        unassignedSensors.Length.Should().Be(2);
    }

    [Fact]
    public async Task ShouldUpdateZone()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

        var updateZoneCommand = new UpdateZoneCommand()
        {
            ZoneId = zoneId,
            Name = TestData.Zones.Kitchen.Name,
            MqttTopic = "SomeTopic",
            Description = TestData.Zones.Kitchen.Description,
            LocationId = locationId
        };

        await client.UpdateZone(updateZoneCommand, token);

        var updatedZone = await client.GetZoneDetails(zoneId, token);

        updatedZone.Name.Should().Be(TestData.Zones.Kitchen.Name);
        updatedZone.Description.Should().Be(TestData.Zones.Kitchen.Description);
        updatedZone.MqttTopic.Should().Be("SomeTopic");
    }

    [Fact]
    public async Task ShouldDeleteZone()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

        (await client.GetZones(locationId, token)).Should().NotBeEmpty();

        await client.DeleteZone(zoneId, token);

        (await client.GetZones(locationId, token)).Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldGetSensorDetails()
    {
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        var sensorDetails = await client.GetSensorDetails(testApplication.LivingRoomSensorId, testApplication.Token);

        sensorDetails.ExternalId.Should().Be(TestData.Sensors.LivingRoomSensor);
    }


    [Fact]
    public async Task ShouldGetZoneInsights()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        var zoneInsights = await client.GetZoneInsights(testApplication.LivingRoomZoneId, TimeRange.Day, testApplication.Token);
    }
}
