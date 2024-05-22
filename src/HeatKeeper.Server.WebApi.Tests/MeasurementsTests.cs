using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Zones.Api;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

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
    public async Task ShouldDeleteExpiredMeasurements()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        await testLocation.DeleteAllMeasurements();
        await client.CreateLivingRoomTemperatureMeasurements(count: 5, TimeSpan.FromMinutes(15), RetentionPolicy.Hour, testLocation.Token);

        var janitor = Factory.Services.GetRequiredService<IJanitor>();
        await janitor.Run("DeleteExpiredMeasurements");

        var zoneInsights = await client.GetZoneInsights(testLocation.LivingRoomZoneId, TimeRange.Day, testLocation.Token);
        zoneInsights.TemperatureMeasurements.Length.Should().Be(5);

        now.Advance(TimeSpan.FromHours(1));

        await janitor.Run("DeleteExpiredMeasurements");

        zoneInsights = await client.GetZoneInsights(testLocation.LivingRoomZoneId, TimeRange.Day, testLocation.Token);
        zoneInsights.TemperatureMeasurements.Length.Should().Be(4);
    }

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
    public async Task ShouldUpdateLastSeenOnSensor()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();
        await testApplication.DeleteAllMeasurements();
        await client.CreateLivingRoomTemperatureMeasurements(count: 5, TimeSpan.FromMinutes(1), RetentionPolicy.Hour, testApplication.Token);

        var sensorDetails = await client.GetSensorDetails(testApplication.LivingRoomSensorId, testApplication.Token);
        sensorDetails.LastSeen.Should().Be(now.GetUtcNow().AddMinutes(4).UtcDateTime);
    }
}
