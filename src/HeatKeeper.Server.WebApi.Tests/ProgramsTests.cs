using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Schedules;
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
        var testLocation = await Factory.CreateTestLocation();

        var programId = await client.CreateProgram(TestData.Programs.TestProgram(testLocation.LocationId), testLocation.Token);

        var programDetails = await client.GetProgramDetails(programId, testLocation.Token);

        programDetails.Name.Should().Be(TestData.Programs.TestProgramName);
        programDetails.Description.Should().Be(TestData.Programs.TestProgramDescription);
        programDetails.ActiveScheduleId.Should().BeNull();
    }

    [Fact]
    public async Task ShouldUpdateProgram()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var programId = await client.CreateProgram(TestData.Programs.TestProgram(testLocation.LocationId), testLocation.Token);
        await client.UpdateProgram(TestData.Programs.UpdatedTestProgram(programId, testLocation.LocationId), testLocation.Token);

        var programDetails = await client.GetProgramDetails(programId, testLocation.Token);
        programDetails.Name.Should().Be(TestData.Programs.TestProgramUpdatedName);
        programDetails.Description.Should().Be(TestData.Programs.TestProgramUpdatedDescription);
    }

    [Fact]
    public async Task ShouldActivateProgram()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var programId = await client.CreateProgram(TestData.Programs.TestProgram(testLocation.LocationId), testLocation.Token);
        await client.ActivateProgram(programId, testLocation.Token);

        var locationDetails = await client.GetLocationDetails(testLocation.LocationId, testLocation.Token);
        locationDetails.ActiveProgramId.Should().Be(programId);
    }

    [Fact]
    public async Task ShouldDeletePrograms()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        // Delete programs from test location that has schedules and setpoints
        var programs = await client.GetPrograms(testLocation.LocationId, testLocation.Token);
        foreach (var program in programs)
        {
            await client.DeleteProgram(program.Id, testLocation.Token);
        }

        programs = await client.GetPrograms(testLocation.LocationId, testLocation.Token);
        programs.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldCreateSchedule()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var scheduleId = await client.CreateSchedule(TestData.Schedules.TestSchedule(testLocation.NormalProgramId), testLocation.Token);

        var scheduleDetails = await client.GetScheduleDetails(scheduleId, testLocation.Token);

        scheduleDetails.Id.Should().Be(scheduleId);
        scheduleDetails.Name.Should().Be(TestData.Schedules.TestScheduleName);
        scheduleDetails.CronExpression.Should().Be(TestData.Schedules.TestScheduleCronExpression);
    }

    [Fact]
    public async Task ShouldUpdateSchedule()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var scheduleId = await client.CreateSchedule(TestData.Schedules.TestSchedule(testLocation.NormalProgramId), testLocation.Token);

        await client.UpdateSchedule(TestData.Schedules.UpdatedSchedule(scheduleId), testLocation.Token);

        var scheduleDetails = await client.GetScheduleDetails(scheduleId, testLocation.Token);

        scheduleDetails.Id.Should().Be(scheduleId);
        scheduleDetails.Name.Should().Be(TestData.Schedules.TestScheduleUpdatedName);
        scheduleDetails.CronExpression.Should().Be(TestData.Schedules.TestScheduleUpdatedCronExpression);
    }

    [Fact]
    public async Task ShouldDeleteSchedules()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var schedules = await client.GetSchedules(testLocation.NormalProgramId, testLocation.Token);

        foreach (var schedule in schedules)
        {
            await client.DeleteSchedule(schedule.Id, testLocation.Token);
        }

        schedules = await client.GetSchedules(testLocation.NormalProgramId, testLocation.Token);
        schedules.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldGetSetPoints()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var setPoints = await client.GetSetPoints(testLocation.ScheduleId, testLocation.Token);

        setPoints.Should().HaveCount(1);
        setPoints.Single().ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        setPoints.Single().Value.Should().Be(TestData.SetPoints.LivingRoomSetPoint);
    }


    [Fact]
    public async Task ShouldCreateSetPoint()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var testZoneId = await client.CreateZone(testLocation.LocationId, TestData.Zones.TestZone, testLocation.Token);

        var createSetPointCommand = TestData.SetPoints.LivingRoom(testLocation.ScheduleId, testZoneId);
        var setPointId = await client.CreateSetPoint(testLocation.ScheduleId, createSetPointCommand, testLocation.Token);


        var setPoint = await client.GetSetPointDetails(setPointId, testLocation.Token);

        setPoint.Id.Should().Be(setPointId);
        setPoint.ScheduleName.Should().Be(TestData.Schedules.DayTimeScheduleName);
        setPoint.ZoneName.Should().Be(TestData.Zones.TestZoneName);
        setPoint.Value.Should().Be(TestData.SetPoints.LivingRoomSetPoint);
        setPoint.Hysteresis.Should().Be(TestData.SetPoints.LivingRoomHysteresis);
    }


    [Fact]
    public async Task ShouldUpdateSetPoint()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.UpdateSetPoint(TestData.SetPoints.UpdatedLivingRoom(testLocation.LivingRoomSetPointId), testLocation.Token);

        var setPoint = await client.GetSetPointDetails(testLocation.LivingRoomSetPointId, testLocation.Token);

        setPoint.Id.Should().Be(testLocation.LivingRoomSetPointId);
        setPoint.Value.Should().Be(TestData.SetPoints.UpdatedLivingRoomSetPoint);
        setPoint.Hysteresis.Should().Be(TestData.SetPoints.UpdatedLivingRoomHysteresis);
    }

    [Fact]
    public async Task ShouldDeleteSetPoints()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var setPoints = await client.GetSetPoints(testLocation.ScheduleId, testLocation.Token);

        foreach (var setPoint in setPoints)
        {
            await client.DeleteSetPoint(setPoint.Id, testLocation.Token);
        }

        setPoints = await client.GetSetPoints(testLocation.ScheduleId, testLocation.Token);
        setPoints.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldGetZonesNotAssignedToSchedule()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var zonesNotAssignedToSchedule = await client.GetZonesNotAssignedToSchedule(testLocation.ScheduleId, testLocation.Token);
        zonesNotAssignedToSchedule.Should().HaveCount(1);
        zonesNotAssignedToSchedule.Single().Name.Should().Be(TestData.Zones.KitchenName);
        zonesNotAssignedToSchedule.Single().Id.Should().Be(testLocation.KitchenZoneId);
    }


    [Fact]
    public async Task ShouldAddSchedulesToJanitor()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var janitor = Factory.Services.GetService<IJanitor>();

        janitor.Should().Contain(j => j.Name == $"Schedule_{testLocation.ScheduleId}");
    }

    [Fact]
    public async Task ShouldAddSchedulesToJanitorWhenScheduleIsAdded()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var testZone = await client.CreateZone(testLocation.LocationId, TestData.Zones.TestZone, testLocation.Token);
        var scheduleId = await client.CreateSchedule(TestData.Schedules.TestSchedule(testLocation.NormalProgramId), testLocation.Token);

        var janitor = Factory.Services.GetService<IJanitor>();
        janitor.Should().Contain(j => j.Name == $"Schedule_{scheduleId}");
    }

    [Fact]
    public async Task ShouldUpdateSchedulesToJanitorWhenScheduleIsUpdated()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.UpdateSchedule(TestData.Schedules.UpdatedSchedule(testLocation.ScheduleId), testLocation.Token);

        var janitor = Factory.Services.GetService<IJanitor>();
        var scheduledTask = janitor.Single(j => j.Name == $"Schedule_{testLocation.ScheduleId}");

        // TODO - Make this public in the ScheduledTask class (Janitor)
        var cronSchedule = (CronSchedule)typeof(ScheduledTask).GetField("_schedule", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scheduledTask);

        cronSchedule.CronScheduleExpression.Should().Be(TestData.Schedules.TestScheduleUpdatedCronExpression);
    }

    [Fact]
    public async Task ShouldRemoveSchedulesFromJanitorWhenScheduleIsDeleted()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.DeleteSchedule(testLocation.ScheduleId, testLocation.Token);
        var janitor = Factory.Services.GetService<IJanitor>();
        janitor.Should().Contain(j => j.Name == $"Schedule_{testLocation.ScheduleId}" && j.State == TaskState.DeleteRequested);
    }


    //     [Fact]
    //     public async Task ShouldAddAndUpdateJanitorWhenScheduleIsInsertedAndUpdated()
    //     {
    //         var client = Factory.CreateClient();
    //         var token = await client.AuthenticateAsAdminUser();

    //         var locationId = await client.CreateLocation(TestData.Locations.Home, token);

    //         var insertProgramCommand = new CreateProgramCommand("Normal", locationId);

    //         var programId = await client.CreateProgram(insertProgramCommand, token);

    //         var dayTimeScheduleCommand = new CreateScheduleCommand(programId, "DayTime", "0 15,18,21 * * *");

    //         var dayTimeScheduleId = await client.CreateSchedule(programId, dayTimeScheduleCommand, token);

    //         var nightTimeScheduleCommand = new CreateScheduleCommand(programId, "NightTime", "0 15,18,21 * * *");

    //         await client.CreateSchedule(programId, nightTimeScheduleCommand, token);

    //         var janitor = Factory.Services.GetService<IJanitor>();

    //         janitor.Count().Should().Be(5);
    //     }
}