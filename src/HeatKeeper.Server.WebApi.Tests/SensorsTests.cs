using System;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Sensors.Api;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class DeadSensorsTests : TestBase
{
    [Fact]
    public async Task ShouldGetDeadSensors()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        var deadSensors = await client.GetDeadSensors(testApplication.Token);

        deadSensors.Should().BeEmpty();

        now.Advance(TimeSpan.FromHours(14));

        deadSensors = await client.GetDeadSensors(testApplication.Token);

        deadSensors.Length.Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateSensor()
    {
        var client = Factory.CreateClient();
        var testApplication = await Factory.CreateTestLocation();

        var sensorDetails = await client.GetSensorDetails(testApplication.LivingRoomSensorId, testApplication.Token);

        await client.UpdateSensor(new UpdateSensorCommand(testApplication.LivingRoomSensorId, "New name", "New description", true, 30), testApplication.Token);

        var updatedSensorDetails = await client.GetSensorDetails(testApplication.LivingRoomSensorId, testApplication.Token);

        updatedSensorDetails.Name.Should().Be("New name");
        updatedSensorDetails.Description.Should().Be("New description");
        updatedSensorDetails.EnableDeadSensorNotification.Should().BeTrue();
        updatedSensorDetails.MinutesBeforeSensorIsConsideredDead.Should().Be(30);
    }
}