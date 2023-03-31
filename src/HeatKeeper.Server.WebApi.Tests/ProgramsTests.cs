using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Abstractions;
using HeatKeeper.Server.Programs;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class ProgramsTests : TestBase
{
    [Fact]
    public async Task ShouldCreateProgram()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        var programId = await client.CreateProgram(insertProgramCommand, token);

        await client.ActivateProgram(programId, token);

        var programs = await client.GetPrograms(locationId, token);

        programs.Length.Should().Be(1);
        programs.Single().Name.Should().Be("Normal");
        programs.Single().ActiveScheduleId.Should().BeNull();

        var scheduleId = await client.CreateSchedule(programId, new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *"), token);

        await client.UpdateProgram(new UpdateProgramCommand(programId, "Away", scheduleId), token);

        programs = await client.GetPrograms(locationId, token);
        programs.Single().Name.Should().Be(expected: "Away");
        programs.Single().ActiveScheduleId.Should().Be(scheduleId);

        await client.DeleteProgram(programId, token);
        programs = await client.GetPrograms(locationId, token);
        programs.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldCreateSchedule()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        var programId = await client.CreateProgram(insertProgramCommand, token);

        var createScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        var scheduleId = await client.CreateSchedule(programId, createScheduleCommand, token);

        var schedules = await client.GetSchedules(programId, token);

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
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        var programId = await client.CreateProgram(insertProgramCommand, token);

        var createScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        var scheduleId = await client.CreateSchedule(programId, createScheduleCommand, token);

        var zoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);

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

    [Fact]
    public async Task ShouldAddSchedulesToJanitorWithBootStrapper()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        var programId = await client.CreateProgram(insertProgramCommand, token);

        var dayTimeScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        var dayTimeScheduleId = await client.CreateSchedule(programId, dayTimeScheduleCommand, token);

        var nightTimeScheduleCommand = new CreateScheduleCommand(programId, "NightTime", "0 15,18,21 * * *");

        var nightTimeScheduleId = await client.CreateSchedule(programId, nightTimeScheduleCommand, token);

        var bootStrapper = Factory.Services.GetServices<IBootStrapper>().Single(s => s.GetType() == typeof(JanitorBootStrapper));

        await bootStrapper.Execute();

        var janitor = Factory.Services.GetService<IJanitor>();

        janitor.Count().Should().Be(5);
    }

    [Fact]
    public async Task ShouldAddAndUpdateJanitorWhenScheduleIsInsertedAndUpdated()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

        var programId = await client.CreateProgram(insertProgramCommand, token);

        var dayTimeScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        var dayTimeScheduleId = await client.CreateSchedule(programId, dayTimeScheduleCommand, token);

        var nightTimeScheduleCommand = new CreateScheduleCommand(programId, "NightTime", "0 15,18,21 * * *");

        await client.CreateSchedule(programId, nightTimeScheduleCommand, token);

        var janitor = Factory.Services.GetService<IJanitor>();

        janitor.Count().Should().Be(5);
    }
}