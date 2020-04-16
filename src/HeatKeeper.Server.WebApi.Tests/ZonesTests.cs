using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Host.Locations;
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
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            await client.GetZones(12, token);

        }

        [Fact]
        public async Task ShouldGetUnassignedSensorsForZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var sensors = await client.GetSensors(zoneId, token);

            sensors.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldAssignSensorToZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);

            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var sensors = await client.GetSensors(zoneId, token);

            await client.AddSensorToZone(zoneId, new AddSensorToZoneCommand() { SensorId = sensors[0].Id }, token);


            sensors = await client.GetSensors(zoneId, token);
            sensors[0].ZoneId.Should().Be(zoneId);
        }

        [Fact]
        public async Task ShouldRemoveSensorFromZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var sensors = await client.GetSensors(zoneId, token);

            await client.AddSensorToZone(zoneId, new AddSensorToZoneCommand() { SensorId = sensors[0].Id }, token);


            sensors = await client.GetSensors(zoneId, token);

            sensors[0].ZoneId.Should().Be(zoneId);

            var removeSensorResponse = await client.RemoveSensorFromZone(new RemoveSensorFromZoneCommand() { SensorId = sensors[0].Id, ZoneId = zoneId }, token);
            removeSensorResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            sensors = await client.GetSensors(zoneId, token);
            sensors[0].ZoneId.Should().BeNull();
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
                Description = TestData.Zones.Kitchen.Description
            };

            await client.UpdateZone(updateZoneCommand, token);

            var updatedZone = (await client.GetZones(locationId, token)).Single();

            updatedZone.Name.Should().Be(TestData.Zones.Kitchen.Name);
            updatedZone.Description.Should().Be(TestData.Zones.Kitchen.Description);
        }
    }
}
