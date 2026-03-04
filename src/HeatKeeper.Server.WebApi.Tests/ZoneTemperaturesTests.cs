using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using HeatKeeper.Server.ZoneTemperatures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ZoneTemperaturesTests : TestBase
{
    // Hour used for test measurements, distinct from the 14:00 hour used by CreateTestLocation
    private static readonly DateTime TestHour = new(1972, 1, 21, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ShouldCreateEntryWhenTemperatureMeasurementIsInserted()
    {
        var testLocation = await Factory.CreateTestLocation();

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 21.5, TestHour.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().ContainSingle(e => e.Hour == TestHour)
            .Which.Temperature.Should().BeApproximately(21.5, 0.001);
    }

    [Fact]
    public async Task ShouldAverageMultipleMeasurementsInSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(15)),
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, TestHour.AddMinutes(45))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().ContainSingle(e => e.Hour == TestHour)
            .Which.Temperature.Should().BeApproximately(22.0, 0.001);
    }

    [Fact]
    public async Task ShouldCreateSeparateEntriesForDifferentHours()
    {
        var testLocation = await Factory.CreateTestLocation();
        var hour2 = TestHour.AddHours(1);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(30)),
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, hour2.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().Contain(e => e.Hour == TestHour).Which.Temperature.Should().BeApproximately(20.0, 0.001);
        entries.Should().Contain(e => e.Hour == hour2).Which.Temperature.Should().BeApproximately(24.0, 0.001);
    }

    [Fact]
    public async Task ShouldAverageAcrossMultipleSensorsInSameZone()
    {
        var testLocation = await Factory.CreateTestLocation();
        const string sensor2 = "ZONE_TEMP_SENSOR2";

        // Register sensor2 with a non-temperature measurement so it doesn't pollute the hourly average
        await testLocation.HttpClient.CreateMeasurements([
            new MeasurementCommand(sensor2, MeasurementType.Humidity, RetentionPolicy.None, 50.0, TestHour.AddMinutes(5))
        ], testLocation.Token);
        var unassigned = await testLocation.HttpClient.GetUnassignedSensors(testLocation.Token);
        await testLocation.HttpClient.AssignZoneToSensor(
            new AssignZoneToSensorCommand(unassigned.Single(s => s.ExternalId == sensor2).Id, testLocation.LivingRoomZoneId),
            testLocation.Token);

        // Now insert temperature measurements from both sensors in the same hour
        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(30)),
            Temp(sensor2, 24.0, TestHour.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().Contain(e => e.Hour == TestHour)
            .Which.Temperature.Should().BeApproximately(22.0, 0.001);
    }

    [Fact]
    public async Task ShouldNotInterfereAcrossZones()
    {
        var testLocation = await Factory.CreateTestLocation();
        const string kitchenSensorId = "ZONE_TEMP_KITCHEN_SENSOR";

        // Register kitchen sensor with a non-temperature measurement so it doesn't pollute the hourly average
        await testLocation.HttpClient.CreateMeasurements([
            new MeasurementCommand(kitchenSensorId, MeasurementType.Humidity, RetentionPolicy.None, 50.0, TestHour.AddMinutes(5))
        ], testLocation.Token);
        var unassigned = await testLocation.HttpClient.GetUnassignedSensors(testLocation.Token);
        await testLocation.HttpClient.AssignZoneToSensor(
            new AssignZoneToSensorCommand(unassigned.Single(s => s.ExternalId == kitchenSensorId).Id, testLocation.KitchenZoneId),
            testLocation.Token);

        // Insert distinct temperatures for each zone
        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(30)),
            Temp(kitchenSensorId, 30.0, TestHour.AddMinutes(30))
        ], testLocation.Token);

        var livingRoom = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);
        var kitchen = await GetZoneTemperatures(testLocation, testLocation.KitchenZoneId);

        livingRoom.Should().Contain(e => e.Hour == TestHour).Which.Temperature.Should().BeApproximately(20.0, 0.001);
        kitchen.Should().Contain(e => e.Hour == TestHour).Which.Temperature.Should().BeApproximately(30.0, 0.001);
    }

    [Fact]
    public async Task ShouldIgnoreNonTemperatureMeasurements()
    {
        var testLocation = await Factory.CreateTestLocation();

        await testLocation.HttpClient.CreateMeasurements([
            new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Humidity, RetentionPolicy.None, 60.0, TestHour.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().NotContain(e => e.Hour == TestHour);
    }

    [Fact]
    public async Task ShouldIgnoreSensorNotAssignedToZone()
    {
        var testLocation = await Factory.CreateTestLocation();
        const string unassignedSensorId = "ZONE_TEMP_UNASSIGNED_SENSOR";

        // Cause sensor creation by submitting a measurement (sensor exists but has no zone)
        await testLocation.HttpClient.CreateMeasurements([
            Temp(unassignedSensorId, 21.5, TestHour.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().NotContain(e => e.Hour == TestHour);
    }

    [Fact]
    public async Task ShouldUpdateExistingEntryWhenNewMeasurementArrivesInSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();

        // First batch: one reading
        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(15))
        ], testLocation.Token);

        // Second batch: another reading in the same hour
        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, TestHour.AddMinutes(45))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        // Only one row for the hour; temperature is the average of both readings
        entries.Where(e => e.Hour == TestHour).Should().ContainSingle()
            .Which.Temperature.Should().BeApproximately(22.0, 0.001);
    }

    [Fact]
    public async Task ShouldSetLastUpdateToLatestMeasurementTimestampInBatch()
    {
        var testLocation = await Factory.CreateTestLocation();
        var early = TestHour.AddMinutes(15);
        var late = TestHour.AddMinutes(45);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, early),
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, late)
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().Contain(e => e.Hour == TestHour)
            .Which.LastUpdate.Should().Be(late);
    }

    [Fact]
    public async Task ShouldUpdateLastUpdateWhenLaterBatchArrivesInSameHour()
    {
        var testLocation = await Factory.CreateTestLocation();
        var firstTime = TestHour.AddMinutes(15);
        var secondTime = TestHour.AddMinutes(45);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, firstTime)
        ], testLocation.Token);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, secondTime)
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().Contain(e => e.Hour == TestHour)
            .Which.LastUpdate.Should().Be(secondTime);
    }

    [Fact]
    public async Task ShouldHandleMultipleBatchesCreatingMultipleHours()
    {
        var testLocation = await Factory.CreateTestLocation();
        var hour2 = TestHour.AddHours(1);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 20.0, TestHour.AddMinutes(30))
        ], testLocation.Token);

        await testLocation.HttpClient.CreateMeasurements([
            Temp(TestData.Sensors.LivingRoomSensor, 24.0, hour2.AddMinutes(30))
        ], testLocation.Token);

        var entries = await GetZoneTemperatures(testLocation, testLocation.LivingRoomZoneId);

        entries.Should().Contain(e => e.Hour == TestHour).Which.Temperature.Should().BeApproximately(20.0, 0.001);
        entries.Should().Contain(e => e.Hour == hour2).Which.Temperature.Should().BeApproximately(24.0, 0.001);
    }

    private async Task<ZoneTemperature[]> GetZoneTemperatures(TestLocation testLocation, long zoneId)
    {
        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();
        return await queryExecutor.ExecuteAsync(new GetZoneTemperaturesQuery(zoneId));
    }

    private static MeasurementCommand Temp(string sensorId, double value, DateTime created)
        => new(sensorId, MeasurementType.Temperature, RetentionPolicy.None, value, created);
}
