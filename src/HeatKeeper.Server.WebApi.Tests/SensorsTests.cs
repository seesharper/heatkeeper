using System;
using HeatKeeper.Server.Sensors.Api;

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

        await client.UpdateSensor(new UpdateSensorCommand(testApplication.LivingRoomSensorId, "New name", "New description", "New ExternalId", 5), testApplication.Token);

        var updatedSensorDetails = await client.GetSensorDetails(testApplication.LivingRoomSensorId, testApplication.Token);

        updatedSensorDetails.Name.Should().Be("New name");
        updatedSensorDetails.Description.Should().Be("New description");
        updatedSensorDetails.ExternalId.Should().Be("New ExternalId");
        updatedSensorDetails.MinutesBeforeConsideredDead.Should().Be(5);

    }
}