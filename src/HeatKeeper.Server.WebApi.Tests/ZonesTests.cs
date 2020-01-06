using System.Threading.Tasks;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Sensors;
using Xunit;
using FluentAssertions;
using System.Net;

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
            await client.GetZones(12);

        }

        [Fact]
        public async Task ShouldGetUnassignedSensorsForZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await createLocationResponse.ContentAs<CreateLocationResponse>()).Id;

            var createZoneRequest = await client.CreateZone(locationId, TestData.Zones.LivingRoom);
            var zoneId = (await createZoneRequest.ContentAs<ResourceId>()).Id;

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var getSensorsResponse = await client.GetSensors(token, zoneId);
            var sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            sensors.Length.Should().Be(1);
        }

        [Fact]
        public async Task ShouldAssignSensorToZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await createLocationResponse.ContentAs<CreateLocationResponse>()).Id;

            var createZoneRequest = await client.CreateZone(locationId, TestData.Zones.LivingRoom);
            var zoneId = (await createZoneRequest.ContentAs<ResourceId>()).Id;

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var getSensorsResponse = await client.GetSensors(token, zoneId);
            var sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            var addSensorResponse = await client.AddSensorToZone(new AddSensorToZoneCommand() { ZoneId = zoneId, SensorId = sensors[0].Id }, token);
            addSensorResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            getSensorsResponse = await client.GetSensors(token, zoneId);
            sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            sensors[0].ZoneId.Should().Be(zoneId);
        }

        [Fact]
        public async Task ShouldRemoveSensorFromZone()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();

            var createLocationResponse = await client.CreateLocation(TestData.Locations.Home, token);
            var locationId = (await createLocationResponse.ContentAs<CreateLocationResponse>()).Id;

            var createZoneRequest = await client.CreateZone(locationId, TestData.Zones.LivingRoom);
            var zoneId = (await createZoneRequest.ContentAs<ResourceId>()).Id;

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, token);

            var getSensorsResponse = await client.GetSensors(token, zoneId);
            var sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            var addSensorResponse = await client.AddSensorToZone(new AddSensorToZoneCommand() { ZoneId = zoneId, SensorId = sensors[0].Id }, token);
            addSensorResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            getSensorsResponse = await client.GetSensors(token, zoneId);
            sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            sensors[0].ZoneId.Should().Be(zoneId);

            var removeSensorResponse = await client.RemoveSensorFromZone(new RemoveSensorFromZoneCommand() { SensorId = sensors[0].Id, ZoneId = zoneId }, token);
            removeSensorResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            getSensorsResponse = await client.GetSensors(token, zoneId);
            sensors = await getSensorsResponse.ContentAs<Sensor[]>();

            sensors[0].ZoneId.Should().BeNull();
        }



    }
}
