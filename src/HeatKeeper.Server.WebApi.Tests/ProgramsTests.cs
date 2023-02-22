using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Programs;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ProgramsTests : TestBase
{
    [Fact]
    public async Task ShouldCreateProgram()
    {
        HttpClient client = Factory.CreateClient();
        string token = await client.AuthenticateAsAdminUser();

        long locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        long programId = await client.CreateProgram(insertProgramCommand, token);

        Program[] programs = await client.GetPrograms(locationId, token);

        programs.Length.Should().Be(1);
    }

    [Fact]
    public async Task ShouldCreateSchedule()
    {
        HttpClient client = Factory.CreateClient();
        string token = await client.AuthenticateAsAdminUser();

        long locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        long programId = await client.CreateProgram(insertProgramCommand, token);

        var createScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        long scheduleId = await client.CreateSchedule(programId, createScheduleCommand, token);

        Schedule[] schedules = await client.GetSchedules(programId, token);

        schedules.Length.Should().Be(1);
        schedules.Single().Id.Should().Be(scheduleId);
        schedules.Single().Name.Should().Be("DayTime");
        schedules.Single().CronExpression.Should().Be("0 15,18,21 * * *");

        await client.UpdateSchedule(new UpdateScheduleCommand(scheduleId, "NightTime", "0 23 * * *"), token);

        schedules = await client.GetSchedules(programId, token);

        schedules.Length.Should().Be(1);
        schedules.Single().Id.Should().Be(scheduleId);
        schedules.Single().Name.Should().Be("NightTime");
        schedules.Single().CronExpression.Should().Be("0 23 * * *");

        var zoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);
        await client.CreateSetPoint(scheduleId, new CreateSetPointCommand(scheduleId, zoneId, 20, 2), token);

        await client.DeleteSchedule(scheduleId, token);

        schedules = await client.GetSchedules(programId, token);
        schedules.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldCreateSetPoint()
    {
        HttpClient client = Factory.CreateClient();
        string token = await client.AuthenticateAsAdminUser();

        long locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        long programId = await client.CreateProgram(insertProgramCommand, token);

        var createScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        long scheduleId = await client.CreateSchedule(programId, createScheduleCommand, token);

        long zoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);

        var createSetPointCommand = new CreateSetPointCommand(scheduleId, zoneId, 20, 2);

        var setPointId = await client.CreateSetPoint(scheduleId, createSetPointCommand, token);

        var setPoints = await client.GetSetPoints(scheduleId, token);

        setPoints.Length.Should().Be(1);
        setPoints.Single().Id.Should().Be(setPointId);
        setPoints.Single().Value.Should().Be(20);
        setPoints.Single().Hysteresis.Should().Be(2);

        await client.UpdateSetPoint(new UpdateSetPointCommand(setPointId, 30, 3), token);

        setPoints = await client.GetSetPoints(scheduleId, token);
        setPoints.Single().Id.Should().Be(setPointId);
        setPoints.Single().Value.Should().Be(30);
        setPoints.Single().Hysteresis.Should().Be(3);

        await client.DeleteSetPoint(setPointId, token);

        setPoints = await client.GetSetPoints(scheduleId, token);

        setPoints.Should().BeEmpty();
    }
}