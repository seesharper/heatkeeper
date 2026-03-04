using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.EnergyCosts.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class GetEnergyCostsPerZoneTests : TestBase
{
    [Fact]
    public async Task ShouldReturnHourlyResolutionForToday()
    {
        var now = DateTime.UtcNow;
        var ctx = await SetupZoneWithEnergyCosts(baseHour: new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(-2));

        var result = await ExecuteQuery(ctx, TimePeriod.Today);

        result.Resolution.Should().Be(Resolution.Hourly);
        result.TimeSeries.Should().HaveCount(2);
        result.TimeSeries.All(e => e.Timestamp.Date == now.Date).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnHourlyResolutionForYesterday()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var ctx = await SetupZoneWithEnergyCosts(baseHour: new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 10, 0, 0, DateTimeKind.Utc));

        var result = await ExecuteQuery(ctx, TimePeriod.Yesterday);

        result.Resolution.Should().Be(Resolution.Hourly);
        result.TimeSeries.Should().HaveCount(2);
        result.TimeSeries.All(e => e.Timestamp.Date == yesterday).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnDailyResolutionForLastWeek()
    {
        var fiveDaysAgo = DateTime.UtcNow.Date.AddDays(-5);
        var ctx = await SetupZoneWithEnergyCosts(baseHour: new DateTime(fiveDaysAgo.Year, fiveDaysAgo.Month, fiveDaysAgo.Day, 10, 0, 0, DateTimeKind.Utc));

        var result = await ExecuteQuery(ctx, TimePeriod.LastWeek);

        result.Resolution.Should().Be(Resolution.Daily);
        result.TimeSeries.Should().HaveCount(1);
        result.TimeSeries[0].Timestamp.Date.Should().Be(fiveDaysAgo);
        result.TimeSeries.Sum(e => e.PowerImport).Should().BeApproximately(3.0, 0.001);
    }

    [Fact]
    public async Task ShouldReturnEmptyWhenNoEnergyCostsInTimeRange()
    {
        // Default baseHour is June 2024 — outside the LastYear (2025) range
        var ctx = await SetupZoneWithEnergyCosts();

        var result = await ExecuteQuery(ctx, TimePeriod.LastYear);

        result.TimeSeries.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldOnlyAggregatesSensorsInTheGivenZone()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var baseHour = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 10, 0, 0, DateTimeKind.Utc);
        var ctx = await SetupZoneWithEnergyCosts(baseHour: baseHour);

        // Add a second sensor in a different zone (same location)
        const string sensor2ExternalId = "SENSOR_2_ZONE_ISOLATION_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 200000, baseHour.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        var otherZoneId = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, otherZoneId), ctx.Token);
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 205000, baseHour.AddMinutes(30)),
        ], ctx.Token);

        // Query only for the first zone — should not include sensor2's 5 kWh
        var result = await ExecuteQuery(ctx, TimePeriod.Yesterday);

        result.TimeSeries.Sum(e => e.PowerImport).Should().BeApproximately(3.0, 0.001);
    }

    private async Task<EnergyCost> ExecuteQuery(ZoneEnergyCostContext ctx, TimePeriod timePeriod)
    {
        var queryExecutor = ctx.ServiceProvider.GetRequiredService<IQueryExecutor>();
        return await queryExecutor.ExecuteAsync(new EnergyCostsPerZoneQuery(ctx.ZoneId, timePeriod));
    }

    private record ZoneEnergyCostContext(
        System.Net.Http.HttpClient Client,
        System.IServiceProvider ServiceProvider,
        string Token,
        long LocationId,
        long ZoneId);

    /// <summary>
    /// Sets up a zone with energy costs spanning two hours.
    /// Hour 0: 2 kWh at 1.50/kWh (market)
    /// Hour 1: 1 kWh at 2.50/kWh (market)
    /// </summary>
    private async Task<ZoneEnergyCostContext> SetupZoneWithEnergyCosts(DateTime? baseHour = null)
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = baseHour ?? new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);

        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.50m, 1.20m);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour.AddHours(1), 2.50m, 2.00m);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5)),
        ], token);

        var zoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassigned = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassigned.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, zoneId), token);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30)),
        ], token);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 103000, hour.AddHours(1).AddMinutes(30)),
        ], token);

        return new ZoneEnergyCostContext(client, testLocation.ServiceProvider, token, testLocation.LocationId, zoneId);
    }

    private static async Task InsertEnergyPrice(System.Net.Http.HttpClient client, string token, long energyPriceAreaId, DateTime hour, decimal priceInLocalCurrency, decimal priceAfterSubsidy)
    {
        var price = priceInLocalCurrency.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var priceSubsidy = priceAfterSubsidy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var sql = $"INSERT INTO EnergyPrices (PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy, PriceInEuro, TimeStart, TimeEnd, Currency, ExchangeRate, VATRate, EnergyPriceAreaId) VALUES ({price}, {priceSubsidy}, 0.10, '{hour:yyyy-MM-dd HH:mm:ss}', '{hour.AddHours(1):yyyy-MM-dd HH:mm:ss}', 'NOK', 1.0, 25.0, {energyPriceAreaId})";
        await client.ExecuteDatabaseQuery(sql, token);
    }
}
