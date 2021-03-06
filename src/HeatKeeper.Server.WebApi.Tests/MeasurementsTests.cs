using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.LightInject;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Export;
using HeatKeeper.Server.Sensors;
using LightInject;
using Vibrant.InfluxDB.Client;
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

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);
        }

        [Fact]
        public async Task ShouldUpdateExportedMeasurements()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);

            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            var outsideZoneId = await client.CreateZone(locationId, TestData.Zones.Outside, token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var sensors = await client.GetSensors(livingRoomZoneId, token);

            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
            var outsideSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);
            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = outsideSensor.Id, Name = outsideSensor.Name, Description = outsideSensor.Description, ZoneId = outsideZoneId }, token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var latestMeasurements = await client.GetLatestMeasurements(10, token);

            latestMeasurements.Should().OnlyContain(m => m.Exported != null);
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

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var sensors = await client.GetSensors(livingRoomZoneId, token);

            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
            var outsideSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);
            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = outsideSensor.Id, Name = outsideSensor.Name, Description = outsideSensor.Description, ZoneId = outsideZoneId }, token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

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

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

            var sensors = await client.GetSensors(livingRoomZoneId, token);
            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

            await client.DeleteSensor(livingroomSensor.Id, token);

            sensors = await client.GetSensors(livingRoomZoneId, token);

            livingroomSensor = sensors.SingleOrDefault(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            livingroomSensor.Should().BeNull();
        }

        [Fact]
        public async Task ShouldExportMeasurementWithRetentionPolicy()
        {
            var client = Factory.CreateClient(c =>
                {
                    c.RegisterCommandInterceptor<ExportMeasurementsCommand, IInfluxClient>(async (command, handler, client, token) =>
                    {

                        // Set the zone to something we can query after the handler has been invoked.
                        var zone = Guid.NewGuid().ToString("N");
                        foreach (var measurementToExport in command.MeasurementsToExport)
                        {
                            measurementToExport.Zone = zone;
                        }

                        await handler.HandleAsync(command);

                        var exportedMeasurementsCount = command.MeasurementsToExport.Count();
                        if (exportedMeasurementsCount > 0)
                        {
                            var measurements = await client.ReadAsync<MeasurementToExport>(InfluxDbConstants.DatabaseName, $"SELECT * FROM Day.HeatKeeperMeasurements WHERE Zone = '{zone}'");
                            measurements.Results.Single().Succeeded.Should().BeTrue();
                            measurements.Results.Single().Series.Single().Rows.Count.Should().Be(exportedMeasurementsCount);
                            measurements = await client.ReadAsync<MeasurementToExport>(InfluxDbConstants.DatabaseName, $"SELECT * FROM Week.HeatKeeperMeasurements WHERE Zone = '{zone}'");
                            measurements.Results.Single().Succeeded.Should().BeTrue();
                            measurements.Results.Single().Series.Single().Rows.Count.Should().Be(exportedMeasurementsCount);
                            measurements = await client.ReadAsync<MeasurementToExport>(InfluxDbConstants.DatabaseName, $"SELECT * FROM Month.HeatKeeperMeasurements WHERE Zone = '{zone}'");
                            measurements.Results.Single().Succeeded.Should().BeTrue();
                            measurements.Results.Single().Series.Single().Rows.Count.Should().Be(exportedMeasurementsCount);
                        }
                    });
                });



            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);
            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            await client.CreateMeasurements(TestData.TemperatureMeasurementRequestsWithRetentionPolicy, token);
            var sensors = await client.GetSensors(zoneId, token);
            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = zoneId }, token);
            await client.CreateMeasurements(TestData.TemperatureMeasurementRequestsWithRetentionPolicy, token);
        }
    }



}
