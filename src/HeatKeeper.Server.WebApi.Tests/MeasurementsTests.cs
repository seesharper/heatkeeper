using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.LightInject;
using FluentAssertions;
using HeatKeeper.Server.Export;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors;
using InfluxDB.Client;
using Janitor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public async Task ShouldUpdateExportedMeasurements2()
        {

        }

        // [Fact]
        // public async Task ShouldUpdateExportedMeasurements()
        // {
        //     var client = Factory.CreateClient();
        //     var testApplication = await Factory.CreateTestLocation();

        //     await client.AssignZoneToSensor(new AssignZoneToSensorCommand(testApplication.l, testApplication.LivingRoomZoneId), testApplication.Token);    

        //     var janitor = Factory.Services.GetService<IJanitor>();            
        //     await janitor.Run("ExportMeasurements");

        //     var latestMeasurements = await client.GetLatestMeasurements(10, testApplication.Token);

        //     latestMeasurements.Should().OnlyContain(m => m.Exported != null);
        // }


        [Fact]
        public async Task ShouldMakeLatestMeasurementAvailableThroughDashboard()
        {
            var client = Factory.CreateClient();
            var testApplication = await Factory.CreateTestLocation();

            var dashboardLocation = (await client.GetDashboardLocations(testApplication.Token)).Single();

            dashboardLocation.InsideTemperature.Should().Be(TestData.Measurements.LivingRoomTemperatureMeasurement.Value);
        }

        [Fact]
        public async Task ShouldDeleteSensor()
        {
            var client = Factory.CreateClient();
            var testApplication = await Factory.CreateTestLocation();

            var livingRoomSensors = await client.GetSensors(testApplication.LivingRoomZoneId, testApplication.Token);
            livingRoomSensors.Length.Should().Be(1);

            await client.DeleteSensor(testApplication.LivingRoomSensorId, testApplication.Token);

            livingRoomSensors = await client.GetSensors(testApplication.LivingRoomZoneId, testApplication.Token);
            livingRoomSensors.Length.Should().Be(0);
        }

        [Fact]
        public async Task ShouldExportMeasurementWithRetentionPolicy()
        {
            bool wasIntercepted = false;

            Factory.ConfigureHostBuilder(builder => builder.ConfigureContainer<IServiceContainer>(c =>
            {
                c.RegisterCommandInterceptor<ExportMeasurementsToInfluxDbCommand, IInfluxDBClient>(async (command, handler, client, token) =>
                   {
                       wasIntercepted = true;
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
                           var query = $$"""
                            from(bucket:"{0}")
                                |> range(start: 0)
                                |> filter(fn: (r) => r["zone"] == "{{zone}}")    
                            """;

                           var queryApi = client.GetQueryApi();
                           var result = await queryApi.QueryAsync(string.Format(query, nameof(RetentionPolicy.Hour)), "heatkeeper");
                           result.First().Records.Count().Should().Be(2);

                           result = await queryApi.QueryAsync(string.Format(query, nameof(RetentionPolicy.Day)), "heatkeeper");
                           result.First().Records.Count().Should().Be(2);

                           result = await queryApi.QueryAsync(string.Format(query, nameof(RetentionPolicy.Week)), "heatkeeper");
                           result.First().Records.Count().Should().Be(2);
                       }
                   });
            }));

            var client = Factory.CreateClient();

            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);
            var locationId = await client.CreateLocation(TestData.Locations.Home, token);
            var zoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
            await client.CreateMeasurements(TestData.TemperatureMeasurementRequestsWithRetentionPolicy, token);
            var sensors = await client.GetUnassignedSensors(token);
            var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);

            await client.AssignZoneToSensor(new AssignZoneToSensorCommand(livingroomSensor.Id, zoneId), token);

            await client.CreateMeasurements(TestData.TemperatureMeasurementRequestsWithRetentionPolicy, token);
            var janitor = Factory.Services.GetService<IJanitor>();
            await janitor.Run("ExportMeasurements");
            wasIntercepted.Should().BeTrue();
        }
    }



}
