using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.EnergyCosts.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using HeatKeeper.Server.ZoneTemperatures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class GetZoneTemperaturesPerZoneTests : TestBase
{
    [Fact]
    public async Task ShouldReturnHourlyResolutionForToday()
    {
        var now = DateTime.UtcNow;
        var ctx = await SetupZoneWithTemperatures(baseHour: new DateTime(now.Year, now.Month, now.Day, Math.Max(0, now.Hour - 3), 0, 0, DateTimeKind.Utc));

        var result = await ExecuteQuery(ctx, TimePeriod.Today);

        result.Resolution.Should().Be(Resolution.Hourly);
        result.TimeSeries.Should().HaveCount(2);
        result.TimeSeries.All(e => e.Timestamp.Date == now.Date).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnHourlyResolutionForYesterday()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var ctx = await SetupZoneWithTemperatures(baseHour: new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 10, 0, 0, DateTimeKind.Utc));

        var result = await ExecuteQuery(ctx, TimePeriod.Yesterday);

        result.Resolution.Should().Be(Resolution.Hourly);
        result.TimeSeries.Should().HaveCount(2);
        result.TimeSeries.All(e => e.Timestamp.Date == yesterday).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnDailyResolutionForLastWeek()
    {
        var fiveDaysAgo = DateTime.UtcNow.Date.AddDays(-5);
        var ctx = await SetupZoneWithTemperatures(baseHour: new DateTime(fiveDaysAgo.Year, fiveDaysAgo.Month, fiveDaysAgo.Day, 10, 0, 0, DateTimeKind.Utc));

        var result = await ExecuteQuery(ctx, TimePeriod.LastWeek);

        result.Resolution.Should().Be(Resolution.Daily);
        result.TimeSeries.Should().HaveCount(1);
        result.TimeSeries[0].Timestamp.Date.Should().Be(fiveDaysAgo);
        result.TimeSeries.Average(e => e.Temperature).Should().BeApproximately(22.0, 0.001);
    }

    [Fact]
    public async Task ShouldReturnEmptyWhenNoTemperaturesInTimeRange()
    {
        // Default baseHour is June 2024 — outside the LastYear (2025) range
        var ctx = await SetupZoneWithTemperatures();

        var result = await ExecuteQuery(ctx, TimePeriod.LastYear);

        result.TimeSeries.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldOnlyIncludeTemperaturesForTheGivenZone()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var baseHour = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 10, 0, 0, DateTimeKind.Utc);
        var ctx = await SetupZoneWithTemperatures(baseHour: baseHour);

        // Add a sensor in a different zone with a different temperature
        const string sensor2ExternalId = "ZONE_TEMP_ISOLATION_SENSOR";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.Temperature, RetentionPolicy.None, 99.0, baseHour.AddMinutes(30)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        var otherZoneId = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, otherZoneId), ctx.Token);
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.Temperature, RetentionPolicy.None, 99.0, baseHour.AddHours(1).AddMinutes(30)),
        ], ctx.Token);

        // Query only the first zone — should not include sensor2's temperature
        var result = await ExecuteQuery(ctx, TimePeriod.Yesterday);

        result.TimeSeries.Should().OnlyContain(e => e.Temperature < 90.0);
    }

    private async Task<ZoneTemperatureResult> ExecuteQuery(ZoneTemperatureContext ctx, TimePeriod timePeriod)
    {
        var queryExecutor = ctx.ServiceProvider.GetRequiredService<IQueryExecutor>();
        return await queryExecutor.ExecuteAsync(new ZoneTemperaturesPerZoneQuery(ctx.ZoneId, timePeriod));
    }

    private record ZoneTemperatureContext(
        System.Net.Http.HttpClient Client,
        System.IServiceProvider ServiceProvider,
        string Token,
        long LocationId,
        long ZoneId);

    /// <summary>
    /// Sets up a zone with two hourly temperature entries.
    /// Hour 0: 20.0°C
    /// Hour 1: 24.0°C
    /// </summary>
    private async Task<ZoneTemperatureContext> SetupZoneWithTemperatures(DateTime? baseHour = null)
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = baseHour ?? new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.None, 20.0, hour.AddMinutes(30)),
        ], token);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.None, 24.0, hour.AddHours(1).AddMinutes(30)),
        ], token);

        return new ZoneTemperatureContext(client, testLocation.ServiceProvider, token, testLocation.LocationId, testLocation.LivingRoomZoneId);
    }
}
