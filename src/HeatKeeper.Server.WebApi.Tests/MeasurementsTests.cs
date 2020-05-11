using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Sensors;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class MeasurementsTests : TestBase
    {
        [Fact]
        public async Task ShouldCreateMeasurementUsingApiKey()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);
        }

        [Fact]
        public async Task ShouldMakeLatestMeasurementAvailableThroughDashboard()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            var outsideZoneId = await client.CreateZone(locationId, TestData.Zones.Outside, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var sensors = await client.GetSensors(livingRoomZoneId, token);

            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
            var outsideSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);
            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = outsideSensor.Id, Name = outsideSensor.Name, Description = outsideSensor.Description, ZoneId = outsideZoneId }, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var dashboardLocation = (await client.GetDashboardLocations(token)).Single();

            dashboardLocation.InsideTemperature.Should().Be(TestData.Measurements.LivingRoomTemperatureMeasurement.Value);
        }

        [Fact]
        public async Task ShouldDeleteSensor()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var sensors = await client.GetSensors(livingRoomZoneId, token);
            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);

            await client.DeleteSensor(livingroomSensor.Id, token);

            sensors = await client.GetSensors(livingRoomZoneId, token);

            livingroomSensor = sensors.SingleOrDefault(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            livingroomSensor.Should().BeNull();
        }


        [Fact]
        public void ShouldUpdateMeasurementAsExported()
        {

        }
    }
}
