using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class GetEnergyCostsTests : TestBase
{
    // ─── Custom period / hourly resolution ───────────────────────────────────

    [Fact]
    public async Task ShouldReturnHourlyResolutionForCustomPeriodWithin24Hours()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        var from = ctx.Hour;
        var to = ctx.Hour.AddHours(4);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        entries.Should().HaveCount(2);
        entries[0].Timestamp.Should().Be(ctx.Hour);
        entries[0].PowerImport.Should().BeApproximately(2.0, 0.001);
        entries[0].CostInLocalCurrency.Should().Be(3.00m);
        entries[1].Timestamp.Should().Be(ctx.Hour.AddHours(1));
        entries[1].PowerImport.Should().BeApproximately(1.0, 0.001);
    }

    [Fact]
    public async Task ShouldReturnDailyResolutionForCustomPeriodExceeding24Hours()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        var from = ctx.Hour.Date;
        var to = ctx.Hour.Date.AddDays(3);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        entries.Should().HaveCount(1);
        entries[0].Timestamp.Should().Be(ctx.Hour.Date);
        entries[0].PowerImport.Should().BeApproximately(3.0, 0.001);
        entries[0].CostInLocalCurrency.Should().Be(5.50m);  // 2*1.50 + 1*2.50
    }

    // ─── TimePeriod: Today / Yesterday ───────────────────────────────────────

    [Fact]
    public async Task ShouldReturnHourlyResolutionForToday()
    {
        var now = DateTime.UtcNow;
        var ctx = await SetupSensorWithEnergyCosts(baseHour: new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(-2));

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Today, ctx.Token);

        entries.Should().HaveCount(2);
        entries.All(e => e.Timestamp.Date == now.Date).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnHourlyResolutionForYesterday()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var ctx = await SetupSensorWithEnergyCosts(baseHour: new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 10, 0, 0, DateTimeKind.Utc));

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Yesterday, ctx.Token);

        entries.Should().HaveCount(2);
        entries.All(e => e.Timestamp.Date == yesterday).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnDailyResolutionForLastWeek()
    {
        var fiveDaysAgo = DateTime.UtcNow.Date.AddDays(-5);
        var ctx = await SetupSensorWithEnergyCosts(baseHour: new DateTime(fiveDaysAgo.Year, fiveDaysAgo.Month, fiveDaysAgo.Day, 10, 0, 0, DateTimeKind.Utc));

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.LastWeek, ctx.Token);

        entries.Should().HaveCount(1);
        entries[0].Timestamp.Date.Should().Be(fiveDaysAgo);
        entries[0].PowerImport.Should().BeApproximately(3.0, 0.001);
    }

    [Fact]
    public async Task ShouldReturnDailyResolutionForThisMonth()
    {
        var now = DateTime.UtcNow;
        var thisMonth = new DateTime(now.Year, now.Month, 5, 10, 0, 0, DateTimeKind.Utc);
        var ctx = await SetupSensorWithEnergyCosts(baseHour: thisMonth);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.ThisMonth, ctx.Token);

        entries.Should().HaveCountGreaterOrEqualTo(1);
        entries.All(e => e.Timestamp.Year == now.Year && e.Timestamp.Month == now.Month).Should().BeTrue();
        entries.Sum(e => e.PowerImport).Should().BeApproximately(3.0, 0.001);
    }

    // ─── SensorId filter ─────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldFilterBySpecificSensor()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Add a second sensor with different costs
        var hour2 = ctx.Hour;
        var zone2Id = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        const string sensor2ExternalId = "SENSOR_2_FILTER_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 50000, hour2.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, zone2Id), ctx.Token);
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 60000, hour2.AddMinutes(30)),
        ], ctx.Token);

        var from = ctx.Hour.AddMinutes(-10);
        var to = ctx.Hour.AddHours(4);

        // Query only for first sensor (ctx.SensorId)
        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, sensorId: ctx.SensorId, fromDateTime: from, toDateTime: to);

        entries.Should().HaveCount(2);
        entries.All(e => e.PowerImport < 5).Should().BeTrue(); // only first sensor's modest costs
        entries.Sum(e => e.PowerImport).Should().BeApproximately(3.0, 0.001);
    }

    // ─── EnergyCalculationStrategy: Sensors (aggregate) ─────────────────────

    [Fact]
    public async Task ShouldAggregateAllSensorsInLocationForSensorsStrategy()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Add a second sensor contributing to the same location
        var zone2Id = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        const string sensor2ExternalId = "SENSOR_2_AGGREGATE_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 200000, ctx.Hour.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, zone2Id), ctx.Token);
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 205000, ctx.Hour.AddMinutes(30)),
        ], ctx.Token);

        var from = ctx.Hour.AddMinutes(-10);
        var to = ctx.Hour.AddHours(1);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        // Should aggregate: sensor1=2kWh + sensor2=5kWh = 7kWh for first hour
        entries.Should().HaveCount(1);
        entries[0].PowerImport.Should().BeApproximately(7.0, 0.001);
    }

    // ─── EnergyCalculationStrategy: SmartMeter ───────────────────────────────

    [Fact]
    public async Task ShouldUseSmartMeterSensorForSmartMeterStrategy()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Add a second sensor (acts as a non-smart-meter sensor)
        var zone2Id = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        const string sensor2ExternalId = "SENSOR_2_SM_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 200000, ctx.Hour.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, zone2Id), ctx.Token);
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 205000, ctx.Hour.AddMinutes(30)),
        ], ctx.Token);

        // Set SmartMeter strategy, designating only the first sensor as the smart meter
        var updateCommand = new UpdateLocationCommand(ctx.LocationId, TestData.Locations.Home.Name, TestData.Locations.Home.Description,
            null, null, TestData.Locations.Home.Longitude, TestData.Locations.Home.Latitude, 0, false, null,
            ctx.SensorId, EnergyCalculationStrategy.SmartMeter);
        await ctx.Client.UpdateLocation(updateCommand, ctx.LocationId, ctx.Token);

        var from = ctx.Hour.AddMinutes(-10);
        var to = ctx.Hour.AddHours(1);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        // Should only include the smart meter sensor (2 kWh), not the second sensor (5 kWh)
        entries.Should().HaveCount(1);
        entries[0].PowerImport.Should().BeApproximately(2.0, 0.001);
    }

    [Fact]
    public async Task ShouldReturnEmptyWhenSmartMeterSensorIdNotSet()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Set SmartMeter strategy without setting a SmartMeterSensorId
        var updateCommand = new UpdateLocationCommand(ctx.LocationId, TestData.Locations.Home.Name, TestData.Locations.Home.Description,
            null, null, TestData.Locations.Home.Longitude, TestData.Locations.Home.Latitude, 0, false, null,
            null, EnergyCalculationStrategy.SmartMeter);
        await ctx.Client.UpdateLocation(updateCommand, ctx.LocationId, ctx.Token);

        var from = ctx.Hour.AddMinutes(-10);
        var to = ctx.Hour.AddHours(4);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        entries.Should().BeEmpty();
    }

    // ─── Edge cases ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnEmptyWhenNoEnergyCostsInTimeRange()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        var from = ctx.Hour.AddDays(10);
        var to = ctx.Hour.AddDays(11);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        entries.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldIncludeCostColumnsForHourlyQuery()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        var from = ctx.Hour;
        var to = ctx.Hour.AddHours(1);

        var entries = await ctx.Client.GetEnergyCosts(ctx.LocationId, TimePeriod.Custom, ctx.Token, fromDateTime: from, toDateTime: to);

        entries.Should().HaveCount(1);
        var entry = entries[0];
        entry.PowerImport.Should().BeApproximately(2.0, 0.001);
        entry.CostInLocalCurrency.Should().Be(3.00m);           // 2 kWh * 1.50
        entry.CostInLocalCurrencyAfterSubsidy.Should().Be(2.40m); // 2 kWh * 1.20
        entry.CostInLocalCurrencyWithFixedPrice.Should().Be(3.00m); // fixed price disabled → same as market
    }

    // ─── Test context / helpers ───────────────────────────────────────────────

    private record EnergyCostContext(
        System.Net.Http.HttpClient Client,
        string Token,
        long LocationId,
        long SensorId,
        DateTime Hour);

    /// <summary>
    /// Sets up a location with energy costs spanning two hours.
    /// Hour 0: 2 kWh at 1.50/kWh (market), 1.20/kWh (subsidy)
    /// Hour 1: 1 kWh at 2.50/kWh (market), 2.00/kWh (subsidy)
    /// </summary>
    private async Task<EnergyCostContext> SetupSensorWithEnergyCosts(DateTime? baseHour = null)
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = baseHour ?? new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);

        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.50m, 1.20m);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour.AddHours(1), 2.50m, 2.00m);

        // Baseline reading before hour starts (creates the sensor)
        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5)),
        ], token);

        // Create zone and assign the power meter sensor
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassigned = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassigned.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Hour 0: +2000 Wh = 2 kWh
        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30)),
        ], token);

        // Hour 1: +1000 Wh = 1 kWh (baseline for hour 1 is 102000 from hour 0)
        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 103000, hour.AddHours(1).AddMinutes(30)),
        ], token);

        return new EnergyCostContext(client, token, testLocation.LocationId, powerMeterSensor.Id, hour);
    }

    private static async Task InsertEnergyPrice(System.Net.Http.HttpClient client, string token, long energyPriceAreaId, DateTime hour, decimal priceInLocalCurrency, decimal priceAfterSubsidy)
    {
        var price = priceInLocalCurrency.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var priceSubsidy = priceAfterSubsidy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var sql = $"INSERT INTO EnergyPrices (PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy, PriceInEuro, TimeStart, TimeEnd, Currency, ExchangeRate, VATRate, EnergyPriceAreaId) VALUES ({price}, {priceSubsidy}, 0.10, '{hour:yyyy-MM-dd HH:mm:ss}', '{hour.AddHours(1):yyyy-MM-dd HH:mm:ss}', 'NOK', 1.0, 25.0, {energyPriceAreaId})";
        await client.ExecuteDatabaseQuery(sql, token);
    }
}
