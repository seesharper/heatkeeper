using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Sensors;

namespace HeatKeeper.Server.WebApi.Tests;

public static class TestApplicationExtensions
{
    public static async Task<TestLocation> CreateTestLocation<TEntryPoint>(this TestApplication<TEntryPoint> testApplication) where TEntryPoint : class
    {
        var client = testApplication.CreateClient();

        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
        var kitchenZoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);


        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

        var sensors = await client.GetSensors(livingRoomZoneId, token);

        var livingRoomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
        await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingRoomSensor.Id, Name = livingRoomSensor.Name, Description = livingRoomSensor.Description, ZoneId = livingRoomZoneId }, token);

        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

        var normalProgramId = await client.CreateProgram(new CreateProgramCommand("Normal", locationId), token);

        await client.ActivateProgram(normalProgramId, token);

        var createScheduleCommand = new CreateScheduleCommand(normalProgramId, "DayTime", "0 15,18,21 * * *");

        var scheduleId = await client.CreateSchedule(normalProgramId, createScheduleCommand, token);

        await client.UpdateProgram(new UpdateProgramCommand(normalProgramId, "Away", scheduleId), token);

        var createSetPointCommand = new CreateSetPointCommand(scheduleId, livingRoomZoneId, 20, 2);

        var setPointId = await client.CreateSetPoint(scheduleId, createSetPointCommand, token);

        return new TestLocation(client, token, locationId, livingRoomZoneId, livingRoomSensor.Id, kitchenZoneId, normalProgramId, scheduleId, setPointId);
    }
}

public record TestLocation(HttpClient HttpClient, string Token, long LocationId, long LivingRoomZoneId, long LivingRoomSensorId, long KitchenZoneId, long NormalProgramId, long ScheduleId, long LivingRoomSetPointId)
{
    public async Task AddLivingRoomMeasurement(double temperature)
    {
        var measurementCommand = new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, temperature, DateTime.UtcNow);
        await HttpClient.CreateMeasurements(new[] { measurementCommand }, Token);
    }
}