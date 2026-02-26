using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class GetEnergyCostsSensorsTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSensorsWithEnergyCostsForLocation()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Add a second sensor with energy costs in the same location
        var zone2Id = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        const string sensor2ExternalId = "SENSOR_2_COSTS_SENSORS_TEST";
        // Baseline reading (creates sensor)
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 50000, ctx.Hour.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, zone2Id), ctx.Token);
        // Second reading triggers energy cost calculation
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 52000, ctx.Hour.AddMinutes(30)),
        ], ctx.Token);

        var sensors = await ctx.Client.GetEnergyCostsSensors(ctx.LocationId, ctx.Token);

        sensors.Should().HaveCount(2);
        sensors.Should().Contain(s => s.Id == ctx.SensorId);
        sensors.Should().Contain(s => s.Id == sensor2.Id);
        sensors.All(s => !string.IsNullOrEmpty(s.Name)).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldNotReturnSensorsFromOtherLocation()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Create a second location and add a sensor with energy costs there
        var location2Id = await ctx.Client.CreateLocation(TestData.Locations.Cabin, ctx.Token);
        await ctx.Client.UpdateLocation(new UpdateLocationCommand(location2Id, TestData.Locations.Cabin.Name, TestData.Locations.Cabin.Description,
            null, null, TestData.Locations.Cabin.Longitude, TestData.Locations.Cabin.Latitude, 0, false, null,
            null, EnergyCalculationStrategy.Sensors), location2Id, ctx.Token);

        var zone2Id = await ctx.Client.CreateZone(location2Id, TestData.Zones.TestZone, ctx.Token);
        const string sensor2ExternalId = "SENSOR_OTHER_LOC_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 50000, ctx.Hour.AddMinutes(-5)),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensor2 = unassigned.Single(s => s.ExternalId == sensor2ExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor2.Id, zone2Id), ctx.Token);

        // Add energy price for location 2's area (uses the same price area from the test location setup)
        await InsertEnergyPrice(ctx.Client, ctx.Token, ctx.PriceAreaId, ctx.Hour, 1.50m, 1.20m);
        // But first update location2 to also use the same price area
        await ctx.Client.UpdateLocation(new UpdateLocationCommand(location2Id, TestData.Locations.Home.Name, TestData.Locations.Home.Description,
            null, null, TestData.Locations.Home.Longitude, TestData.Locations.Home.Latitude, 0, false, ctx.PriceAreaId,
            null, EnergyCalculationStrategy.Sensors), location2Id, ctx.Token);

        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensor2ExternalId, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 52000, ctx.Hour.AddMinutes(30)),
        ], ctx.Token);

        var sensors = await ctx.Client.GetEnergyCostsSensors(ctx.LocationId, ctx.Token);

        sensors.Should().HaveCount(1);
        sensors.Should().Contain(s => s.Id == ctx.SensorId);
        sensors.Should().NotContain(s => s.Id == sensor2.Id);
    }

    [Fact]
    public async Task ShouldNotReturnSensorsWithoutEnergyCosts()
    {
        var ctx = await SetupSensorWithEnergyCosts();

        // Create a second sensor assigned to the location but with no power import measurements
        var zone2Id = await ctx.Client.CreateZone(ctx.LocationId, TestData.Zones.TestZone, ctx.Token);
        const string sensorNoDataExternalId = "SENSOR_NO_COSTS_TEST";
        await ctx.Client.CreateMeasurements([
            new MeasurementCommand(sensorNoDataExternalId, MeasurementType.Temperature, RetentionPolicy.Day, 21.5, ctx.Hour),
        ], ctx.Token);
        var unassigned = await ctx.Client.GetUnassignedSensors(ctx.Token);
        var sensorNoData = unassigned.Single(s => s.ExternalId == sensorNoDataExternalId);
        await ctx.Client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensorNoData.Id, zone2Id), ctx.Token);

        var sensors = await ctx.Client.GetEnergyCostsSensors(ctx.LocationId, ctx.Token);

        sensors.Should().HaveCount(1);
        sensors.Should().Contain(s => s.Id == ctx.SensorId);
        sensors.Should().NotContain(s => s.Id == sensorNoData.Id);
    }

    // ─── Test context / helpers ───────────────────────────────────────────────

    private record EnergyCostContext(
        System.Net.Http.HttpClient Client,
        string Token,
        long LocationId,
        long SensorId,
        long PriceAreaId,
        DateTime Hour);

    private async Task<EnergyCostContext> SetupSensorWithEnergyCosts()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);

        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.50m, 1.20m);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5)),
        ], token);

        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassigned = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassigned.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        await client.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30)),
        ], token);

        return new EnergyCostContext(client, token, testLocation.LocationId, powerMeterSensor.Id, testLocation.PriceAreaId, hour);
    }

    private static async Task InsertEnergyPrice(System.Net.Http.HttpClient client, string token, long energyPriceAreaId, DateTime hour, decimal priceInLocalCurrency, decimal priceAfterSubsidy)
    {
        var price = priceInLocalCurrency.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var priceSubsidy = priceAfterSubsidy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var sql = $"INSERT INTO EnergyPrices (PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy, PriceInEuro, TimeStart, TimeEnd, Currency, ExchangeRate, VATRate, EnergyPriceAreaId) VALUES ({price}, {priceSubsidy}, 0.10, '{hour:yyyy-MM-dd HH:mm:ss}', '{hour.AddHours(1):yyyy-MM-dd HH:mm:ss}', 'NOK', 1.0, 25.0, {energyPriceAreaId})";
        await client.ExecuteDatabaseQuery(sql, token);
    }
}
