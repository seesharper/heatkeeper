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

            await client.AddSensorToZone(livingRoomZoneId, new AddSensorToZoneCommand() { SensorId = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor).Id }, token);
            await client.AddSensorToZone(outsideZoneId, new AddSensorToZoneCommand() { SensorId = sensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor).Id }, token);

            await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var dashboardLocation = (await client.GetDashboardLocations(token)).Single();

            dashboardLocation.InsideTemperature.Should().Be(TestData.Measurements.LivingRoomTemperatureMeasurement.Value);
        }
    }
}
