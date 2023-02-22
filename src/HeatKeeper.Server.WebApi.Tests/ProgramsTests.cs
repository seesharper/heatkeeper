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

        var createSetPointCommand = new CreateSetPointCommand(scheduleId, 20, 2);

        var setPointId = await client.CreateSetPoint(scheduleId, createSetPointCommand, token);

        var setPoints = await client.GetSetPoints(scheduleId, token);

        setPoints.Length.Should().Be(1);
        setPoints.Single().Id.Should().Be(scheduleId);
    }
}