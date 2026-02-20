using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Heaters;
using HeatKeeper.Server.Heaters.Api;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class HeatersTests : TestBase
{
    [Fact]
    public async Task ShouldGetHeaters()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heaters = await client.GetHeaters(testLocation.LivingRoomZoneId, testLocation.Token);
        heaters.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldAddHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heaterId = await client.CreateHeater(TestData.Heaters.TestHeater(testLocation.LivingRoomZoneId), testLocation.Token);

        var heater = await client.GetHeatersDetails(heaterId, testLocation.Token);

        heater.Name.Should().Be(TestData.Heaters.TestHeaterName);
        heater.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        heater.Description.Should().Be(TestData.Heaters.TestHeaterDescription);
        heater.MqttTopic.Should().Be(TestData.Heaters.TestHeaterMqttTopic);
        heater.OnPayload.Should().Be(TestData.Heaters.TestHeaterOnPayload);
        heater.OffPayload.Should().Be(TestData.Heaters.TestHeaterOffPayload);
    }

    [Fact]
    public async Task ShouldUpdateHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.UpdateHeater(TestData.Heaters.UpdateHeater(testLocation.LivingRoomHeaterId1), testLocation.Token);

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);


        heater.Name.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterName);
        heater.ZoneName.Should().Be(TestData.Zones.LivingRoomName);
        heater.Description.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterDescription);
        heater.MqttTopic.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterMqttTopic);
        heater.OnPayload.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterOnPayload);
        heater.OffPayload.Should().Be(TestData.Heaters.UpdatedLivingRoomHeaterOffPayload);
    }

    [Fact]
    public async Task ShouldDeleteHeater()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        await client.DeleteHeater(testLocation.LivingRoomHeaterId1, testLocation.Token);

        var heaters = await client.GetHeaters(testLocation.LivingRoomZoneId, testLocation.Token);
        heaters.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldDisableAndEnableHeaterUsingCommands()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Idle);

        await Factory.Services.GetRequiredService<ICommandHandler<DisableHeaterCommand>>()
            .HandleAsync(new DisableHeaterCommand(testLocation.LivingRoomHeaterId1, HeaterDisabledReason.User));
        heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);

        heater.HeaterState.Should().Be(HeaterState.Disabled);

        await Factory.Services.GetRequiredService<ICommandHandler<EnableHeaterCommand>>()
            .HandleAsync(new EnableHeaterCommand(testLocation.LivingRoomHeaterId1));
        heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);

        heater.HeaterState.Should().Be(HeaterState.Idle);
    }

    [Fact]
    public async Task ShouldDisableAndEnableHeaterUsingEndpoints()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Idle);

        await client.Patch(new UpdateHeaterCommand(testLocation.LivingRoomHeaterId1, heater.Name, heater.Description, heater.MqttTopic, heater.OnPayload, heater.OffPayload, HeaterState.Disabled), testLocation.Token);
        heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);

        heater.HeaterState.Should().Be(HeaterState.Disabled);

        await client.Patch(new UpdateHeaterCommand(testLocation.LivingRoomHeaterId1, heater.Name, heater.Description, heater.MqttTopic, heater.OnPayload, heater.OffPayload, HeaterState.Idle), testLocation.Token);
        heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);

        heater.HeaterState.Should().Be(HeaterState.Idle);
    }

    [Fact]
    public async Task ShouldGetHeaterDisabledReasons()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();

        var reasons = await client.GetHeaterDisabledReasons(testLocation.Token);

        reasons.Should().HaveCount(4);
        reasons.Should().ContainSingle(r => r.Id == 0 && r.Name == "No specific reason or heater is enabled.");
        reasons.Should().ContainSingle(r => r.Id == 1 && r.Name == "Heater was disabled because of a dead sensor.");
        reasons.Should().ContainSingle(r => r.Id == 2 && r.Name == "Heater was manually disabled by the user.");
        reasons.Should().ContainSingle(r => r.Id == 3 && r.Name == "Heater was disabled to prevent overload.");
    }
}