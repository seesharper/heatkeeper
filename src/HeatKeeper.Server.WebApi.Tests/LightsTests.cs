using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Lights;
using HeatKeeper.Server.Lights.Api;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class LightsTests : TestBase
{
    [Fact]
    public async Task ShouldGetLights()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var lights = await client.GetLights(testLocation.LivingRoomZoneId, testLocation.Token);
        lights.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldAddLight()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var lightId = await client.CreateLight(TestData.Lights.TestLight(testLocation.LivingRoomZoneId), testLocation.Token);

        var light = await client.GetLightsDetails(lightId, testLocation.Token);

        light.Name.Should().Be(TestData.Lights.TestLightName);
        light.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        light.Description.Should().Be(TestData.Lights.TestLightDescription);
        light.MqttTopic.Should().Be(TestData.Lights.TestLightMqttTopic);
        light.OnPayload.Should().Be(TestData.Lights.TestLightOnPayload);
        light.OffPayload.Should().Be(TestData.Lights.TestLightOffPayload);
    }

    [Fact]
    public async Task ShouldUpdateLight()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.UpdateLight(TestData.Lights.UpdateLight(testLocation.LivingRoomLightId1), testLocation.Token);

        var light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);

        light.Name.Should().Be(TestData.Lights.UpdatedLivingRoomLightName);
        light.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        light.Description.Should().Be(TestData.Lights.UpdatedLivingRoomLightDescription);
        light.MqttTopic.Should().Be(TestData.Lights.UpdatedLivingRoomLightMqttTopic);
        light.OnPayload.Should().Be(TestData.Lights.UpdatedLivingRoomLightOnPayload);
        light.OffPayload.Should().Be(TestData.Lights.UpdatedLivingRoomLightOffPayload);
    }

    [Fact]
    public async Task ShouldDeleteLight()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.DeleteLight(testLocation.LivingRoomLightId1, testLocation.Token);

        var lights = await client.GetLights(testLocation.LivingRoomZoneId, testLocation.Token);
        lights.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldDisableAndEnableLightUsingCommands()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);
        light.Enabled.Should().BeTrue();

        await Factory.Services.GetRequiredService<ICommandHandler<DisableLightCommand>>()
            .HandleAsync(new DisableLightCommand(testLocation.LivingRoomLightId1));
        light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);

        light.Enabled.Should().BeFalse();

        await Factory.Services.GetRequiredService<ICommandHandler<EnableLightCommand>>()
            .HandleAsync(new EnableLightCommand(testLocation.LivingRoomLightId1));
        light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);

        light.Enabled.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldDisableAndEnableLightUsingEndpoints()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);
        light.Enabled.Should().BeTrue();

        await client.Patch(new UpdateLightCommand(testLocation.LivingRoomLightId1, light.Name, light.Description, light.MqttTopic, light.OnPayload, light.OffPayload, false), testLocation.Token);
        light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);

        light.Enabled.Should().BeFalse();

        await client.Patch(new UpdateLightCommand(testLocation.LivingRoomLightId1, light.Name, light.Description, light.MqttTopic, light.OnPayload, light.OffPayload, true), testLocation.Token);
        light = await client.GetLightsDetails(testLocation.LivingRoomLightId1, testLocation.Token);

        light.Enabled.Should().BeTrue();
    }
}
