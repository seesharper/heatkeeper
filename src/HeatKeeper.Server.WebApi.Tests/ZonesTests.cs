using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Zones;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
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

            var sensors = await client.GetSensors(zoneId, token);

            sensors.Length.Should().Be(2);
        }

        [Fact]
        public async Task ShouldAssignSensorToZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

            var sensors = await client.GetSensors(zoneId, token);

            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = zoneId }, token);

            sensors = await client.GetSensors(zoneId, token);
            livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
            livingroomSensor.ZoneId.Should().Be(zoneId);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = null }, token);

            sensors = await client.GetSensors(zoneId, token);
            livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
            livingroomSensor.ZoneId.Should().BeNull();

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
    }
}
