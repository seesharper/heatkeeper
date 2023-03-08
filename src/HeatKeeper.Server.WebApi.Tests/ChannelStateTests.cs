using System.Threading.Tasks;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Zones;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class ChannelStateTests : TestBase
{
    [Fact]
    public async Task ShouldSetChannelOn()
    {
        var client = Factory.CreateClient();

        var token = await client.AuthenticateAsAdminUser();

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);

        var zoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);

        var programId = await client.CreateProgram(new CreateProgramCommand("Normal", locationId), token);

        await client.ActivateProgram(programId, token);

        var createScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

        long scheduleId = await client.CreateSchedule(programId, createScheduleCommand, token);

        await client.ActivateSchedule(scheduleId, token);

        var createSetPointCommand = new CreateSetPointCommand(scheduleId, zoneId, 20, 2);

        var setPointId = await client.CreateSetPoint(scheduleId, createSetPointCommand, token);

        var janitor = Factory.Services.GetService<IJanitor>();

        await janitor.Run("SetChannelStates");
    }
}