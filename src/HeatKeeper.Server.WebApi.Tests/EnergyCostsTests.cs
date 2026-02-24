using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class EnergyCostsTests : TestBase
{
    [Fact]
    public async Task ShouldCalculateEnergyCostForSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.50m, 1.20m);

        // Baseline measurement before the hour starts (also creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-10));
        await client.CreateMeasurements([m1], token);

        // Create zone and assign the now-existing PM sensor
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Second cumulative reading in the same hour (delta = 2000 Wh = 2 kWh)
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT SensorId, PowerImport, CostInLocalCurrency, CostInLocalCurrencyAfterSubsidy, Hour FROM EnergyCosts ORDER BY Hour", token);
        table.Rows.Length.Should().Be(1);
        Convert.ToDouble(table.Rows[0].Cells[1].Value).Should().Be(2.0); // 2 kWh
        Convert.ToDecimal(table.Rows[0].Cells[2].Value).Should().Be(3.00m); // 2 kWh * 1.50
        Convert.ToDecimal(table.Rows[0].Cells[3].Value).Should().Be(2.40m); // 2 kWh * 1.20
    }

    [Fact]
    public async Task ShouldUpsertEnergyCostForMultipleMeasurementsInSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.00m, 0.80m);

        // Baseline measurement before the hour starts (creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-10));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Second measurement: 1 kWh since baseline
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 101000, hour.AddMinutes(20));
        await client.CreateMeasurements([m2], token);

        // Third measurement in the same hour: 1.5 kWh since baseline (not just 0.5 kWh since m2)
        var m3 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 101500, hour.AddMinutes(40));
        await client.CreateMeasurements([m3], token);

        var table = await client.ExecuteDatabaseQuery("SELECT SensorId, PowerImport, CostInLocalCurrency, Hour FROM EnergyCosts ORDER BY Hour", token);
        // Should still be 1 row (upserted) - accumulated total from the hour baseline
        table.Rows.Length.Should().Be(1);
        // The last upsert reflects the full 1.5 kWh consumed since the hour started
        Convert.ToDouble(table.Rows[0].Cells[1].Value).Should().Be(1.5);
    }

    [Fact]
    public async Task ShouldAttributeCostToCurrentHourUsingReferenceBeforeHourStart()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour10 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var hour12 = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour10, 1.00m, 0.80m);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour12, 3.00m, 2.40m);

        // Baseline at 10:30 (also creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour10.AddMinutes(30));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Reading at 12:15 - reference is the latest reading before 12:00 (m1 at 10:30)
        // Delta = 6000 Wh = 6 kWh, attributed entirely to hour 12
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 106000, hour12.AddMinutes(15));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT PowerImport, CostInLocalCurrency, Hour FROM EnergyCosts ORDER BY Hour", token);
        table.Rows.Length.Should().Be(1);

        // Hour 12: 6 kWh * 3.00 = 18.00
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().Be(6.0);
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(18.00m);
    }

    [Fact]
    public async Task ShouldIgnoreNonCumulativeMeasurements()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        // Send only temperature measurements
        var m1 = new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, 23.5, DateTime.UtcNow);
        await client.CreateMeasurements([m1], token);

        var table = await client.ExecuteDatabaseQuery("SELECT * FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(0);
    }

    [Fact]
    public async Task ShouldNotCreateEnergyCostForFirstMeasurement()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.00m, 0.80m);

        // Send baseline to create the sensor
        var m0 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 90000, hour.AddMinutes(-60));
        await client.CreateMeasurements([m0], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Delete all energy costs that might have been created so far
        await client.ExecuteDatabaseQuery("DELETE FROM EnergyCosts", token);

        // Delete all measurements so the next one has no previous reading
        await client.ExecuteDatabaseQuery("DELETE FROM Measurements WHERE MeasurementType = 12", token);

        // Only one measurement - no previous reading to compute delta
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(15));
        await client.CreateMeasurements([m1], token);

        var table = await client.ExecuteDatabaseQuery("SELECT * FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(0);
    }

    [Fact]
    public async Task ShouldNotCreateEnergyCostWhenSensorNotAssignedToZone()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        // Use a sensor that is not assigned to any zone
        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var m1 = new MeasurementCommand("UNASSIGNED_SENSOR", MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(5));
        var m2 = new MeasurementCommand("UNASSIGNED_SENSOR", MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30));
        await client.CreateMeasurements([m1, m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT * FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(0);
    }

    [Fact]
    public async Task ShouldNotCreateEnergyCostWhenLocationHasNoEnergyPriceArea()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Create a location WITHOUT assigning an EnergyPriceAreaId
        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var zoneId = await client.CreateZone(locationId, TestData.Zones.PowerMeter, token);

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Send first measurement to create the sensor
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(5));
        await client.CreateMeasurements([m1], token);

        // Assign the newly created sensor to the zone
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, zoneId), token);

        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30));
        await client.CreateMeasurements([m2], token);

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var table = await client.ExecuteDatabaseQuery("SELECT * FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(0);
    }

    [Fact]
    public async Task ShouldCreateEnergyCostRecordForZeroAndNegativeDelta()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.00m, 0.80m);

        // Baseline measurement before the hour starts (creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Same value (zero delta) - record should still be upserted with PowerImport = 0
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(20));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT PowerImport, CostInLocalCurrency FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().Be(0.0);
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(0.00m);

        // Negative delta (e.g. meter reset) - record should still be upserted
        var m3 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 50000, hour.AddMinutes(40));
        await client.CreateMeasurements([m3], token);

        table = await client.ExecuteDatabaseQuery("SELECT PowerImport FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().BeLessThan(0.0);
    }

    [Fact]
    public async Task ShouldUseFixedEnergyPriceWhenEnabled()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        // Enable fixed energy price on the location
        var updateCommand = new UpdateLocationCommand(
            testLocation.LocationId,
            TestData.Locations.Home.Name,
            TestData.Locations.Home.Description,
            null,
            testLocation.LivingRoomZoneId,
            TestData.Locations.Home.Longitude,
            TestData.Locations.Home.Latitude,
            0.75,  // fixed price per kWh
            true,  // useFixedEnergyPrice = true
            testLocation.PriceAreaId);
        await client.UpdateLocation(updateCommand, testLocation.LocationId, token);

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 2.00m, 1.60m);

        // Baseline measurement before the hour starts (creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Delta = 4000 Wh = 4 kWh
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 104000, hour.AddMinutes(30));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT CostInLocalCurrency, CostInLocalCurrencyAfterSubsidy, CostInLocalCurrencyWithFixedPrice FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);

        // Market price: 4 kWh * 2.00 = 8.00
        Convert.ToDecimal(table.Rows[0].Cells[0].Value).Should().Be(8.00m);
        // Subsidy price: 4 kWh * 1.60 = 6.40
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(6.40m);
        // Fixed price: 4 kWh * 0.75 = 3.00
        Convert.ToDecimal(table.Rows[0].Cells[2].Value).Should().Be(3.00m);
    }

    [Fact]
    public async Task ShouldFallbackToMarketPriceWhenFixedPriceDisabled()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.50m, 1.20m);

        // Baseline measurement before the hour starts (creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // Delta = 3000 Wh = 3 kWh
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 103000, hour.AddMinutes(30));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT CostInLocalCurrency, CostInLocalCurrencyWithFixedPrice FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);

        // Market price: 3 kWh * 1.50 = 4.50
        Convert.ToDecimal(table.Rows[0].Cells[0].Value).Should().Be(4.50m);
        // Fixed price disabled: should fall back to CostInLocalCurrency = 4.50
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(4.50m);
    }

    [Fact(Skip = "Service temporarily unavailable")]
    public async Task ShouldHandleMissingEnergyPricesWithZeroCost()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        // Use a date far in the past where no energy prices exist and import will fail
        var hour = new DateTime(1970, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        // Baseline measurement before the hour starts (creates the PM sensor)
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-5));
        await client.CreateMeasurements([m1], token);

        // Assign sensor to zone
        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102000, hour.AddMinutes(30));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT PowerImport, CostInLocalCurrency, CostInLocalCurrencyAfterSubsidy FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);

        // Power import is still calculated: 2 kWh
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().Be(2.0);
        // Costs should be 0 since no energy prices available
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(0m);
        Convert.ToDecimal(table.Rows[0].Cells[2].Value).Should().Be(0m);
    }

    [Fact]
    public async Task ShouldAccumulateEnergyCostForMultipleMeasurementsInSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;

        var hour = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        await InsertEnergyPrice(client, token, testLocation.PriceAreaId, hour, 1.00m, 0.80m);

        // Baseline reading before the hour starts - this is the fixed reference for the entire hour
        var m1 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 100000, hour.AddMinutes(-10));
        await client.CreateMeasurements([m1], token);

        var powerMeterZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.PowerMeter, token);
        var unassignedSensors = await client.GetUnassignedSensors(token);
        var powerMeterSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(powerMeterSensor.Id, powerMeterZoneId), token);

        // First reading in the hour: 1 kWh since baseline
        var m2 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 101000, hour.AddMinutes(15));
        await client.CreateMeasurements([m2], token);

        var table = await client.ExecuteDatabaseQuery("SELECT PowerImport, CostInLocalCurrency FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().Be(1.0);
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(1.00m);

        // Second reading in the same hour: 2.5 kWh since baseline (not just 1.5 kWh since m2)
        var m3 = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 102500, hour.AddMinutes(45));
        await client.CreateMeasurements([m3], token);

        table = await client.ExecuteDatabaseQuery("SELECT PowerImport, CostInLocalCurrency FROM EnergyCosts", token);
        table.Rows.Length.Should().Be(1);
        Convert.ToDouble(table.Rows[0].Cells[0].Value).Should().Be(2.5);
        Convert.ToDecimal(table.Rows[0].Cells[1].Value).Should().Be(2.50m);
    }

    private static async Task InsertEnergyPrice(System.Net.Http.HttpClient client, string token, long energyPriceAreaId, DateTime hour, decimal priceInLocalCurrency, decimal priceAfterSubsidy)
    {
        var price = priceInLocalCurrency.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var priceSubsidy = priceAfterSubsidy.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var sql = $"INSERT INTO EnergyPrices (PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy, PriceInEuro, TimeStart, TimeEnd, Currency, ExchangeRate, VATRate, EnergyPriceAreaId) VALUES ({price}, {priceSubsidy}, 0.10, '{hour:yyyy-MM-dd HH:mm:ss}', '{hour.AddHours(1):yyyy-MM-dd HH:mm:ss}', 'NOK', 1.0, 25.0, {energyPriceAreaId})";
        await client.ExecuteDatabaseQuery(sql, token);
    }
}
