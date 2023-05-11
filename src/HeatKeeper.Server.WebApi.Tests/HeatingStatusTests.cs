using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Sensors;
using Janitor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class HeatingStatusTests : TestBase
{
    [Fact]
    public async Task ShouldSetHeatingStatusOn()
    {
        Mock<ICommandHandler<SetZoneHeatingStatusCommand>> setZoneHeatingStatusCommandHandlerMock = new Mock<ICommandHandler<SetZoneHeatingStatusCommand>>();


        var testLocation = await CreateTestLocation();
        var janitor = Factory.Services.GetRequiredService<IJanitor>();
        await testLocation.AddLivingRoomMeasurement(10);
        await janitor.Run("SetChannelStates");
        await testLocation.AddLivingRoomMeasurement(20);
        await janitor.Run("SetChannelStates");

    }

    private async Task<TestLocation> CreateTestLocation()
    {
        var client = Factory.CreateClient(c => c.Mock<SetZoneHeatingStatusCommand>());

        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

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

        return new TestLocation(client, token, locationId, livingRoomZoneId, livingRoomSensor.Id, normalProgramId, scheduleId, setPointId);
    }
}

public record TestLocation(HttpClient HttpClient, string Token, long LocationId, long LivingRoomZoneId, long LivingRoomSensorId, long NormalProgramId, long ScheduleId, long LivingRoomSetPointId)
{
    public async Task AddLivingRoomMeasurement(double temperature)
    {
        var measurementCommand = new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, RetentionPolicy.Day, temperature, DateTime.UtcNow);
        await HttpClient.CreateMeasurements(new[] { measurementCommand }, Token);
    }
}

public static class MockExtensions
{
    public static Mock<T> RegisterMock<T>(this IServiceRegistry registry) where T : class
    {
        Mock<T> mock = new Mock<T>();
        registry.RegisterInstance(mock.Object);
        return mock;
    }

    public static Mock<ICommandHandler<T>> Mock<T>(this IServiceRegistry registry)
    {
        Mock<ICommandHandler<T>> mock = new Mock<ICommandHandler<T>>();
        registry.RegisterInstance(mock.Object);
        return mock;
    }


}

