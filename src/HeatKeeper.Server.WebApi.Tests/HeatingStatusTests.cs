using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using CQRS.Command.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Heaters;
using HeatKeeper.Server.Mqtt;
using HeatKeeper.Server.Programs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class HeatingStatusTests : TestBase
{
    [Fact]
    public async Task ShouldSetHeatingStatusAccordingToSetPoint()
    {
        // var setZoneHeatingStatusCommandHandlerMock = Factory.MockCommandHandler<SetZoneHeatingStatusCommand>();
        // var exportHeatingStatusToInfluxDbCommandHandlerMock = Factory.MockCommandHandler<ExportHeatingStatusToInfluxDbCommand>();
        // var testLocation = await Factory.CreateTestLocation();
        // var janitor = Factory.Services.GetRequiredService<IJanitor>();

        // //Note: The setpoint is 20 degrees, so the heating status should be on when the temperature is below 20 degrees minus the hysteresis which is 2 degree

        // await testLocation.AddLivingRoomMeasurement(10);
        // await janitor.Run("SetChannelStates");
        // setZoneHeatingStatusCommandHandlerMock.VerifyCommandHandler(c => c.HeatingStatus == HeatingStatus.On, Times.Once());
        // exportHeatingStatusToInfluxDbCommandHandlerMock.VerifyCommandHandler(c => c.HeatingStatus == HeatingStatus.On, Times.Once());
        // //Note: The setpoint is 20 degrees, so the heating status should be off when the temperature is above 20 degrees plus the hysteresis which is 2 degree

        // await testLocation.AddLivingRoomMeasurement(30);
        // await janitor.Run("SetChannelStates");
        // setZoneHeatingStatusCommandHandlerMock.VerifyCommandHandler(c => c.HeatingStatus == HeatingStatus.Off, Times.Once());
        // exportHeatingStatusToInfluxDbCommandHandlerMock.VerifyCommandHandler(c => c.HeatingStatus == HeatingStatus.Off, Times.Once());
    }

    [Fact]
    public async Task ShouldSetHeaterStateToActiveWhenHeatingTurnsOn()
    {
        Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var handler = Factory.Services.GetRequiredService<ICommandHandler<SetZoneHeatingStatusCommand>>();

        await handler.HandleAsync(new SetZoneHeatingStatusCommand(testLocation.LivingRoomZoneId, HeatingStatus.On));

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Active);
    }

    [Fact]
    public async Task ShouldSetHeaterStateToIdleWhenHeatingTurnsOff()
    {
        Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var handler = Factory.Services.GetRequiredService<ICommandHandler<SetZoneHeatingStatusCommand>>();

        await handler.HandleAsync(new SetZoneHeatingStatusCommand(testLocation.LivingRoomZoneId, HeatingStatus.On));
        await handler.HandleAsync(new SetZoneHeatingStatusCommand(testLocation.LivingRoomZoneId, HeatingStatus.Off));

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Idle);
    }

    [Fact]
    public async Task ShouldNotChangeStateOfPausedHeaterWhenHeatingTurnsOff()
    {
        Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var setStateHandler = Factory.Services.GetRequiredService<ICommandHandler<SetHeaterStateCommand>>();
        var handler = Factory.Services.GetRequiredService<ICommandHandler<SetZoneHeatingStatusCommand>>();

        await setStateHandler.HandleAsync(new SetHeaterStateCommand(testLocation.LivingRoomHeaterId1, HeaterState.Paused));
        await handler.HandleAsync(new SetZoneHeatingStatusCommand(testLocation.LivingRoomZoneId, HeatingStatus.Off));

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Paused);
    }

    [Fact]
    public async Task ShouldNotChangeStateOfDisabledHeaterWhenHeatingTurnsOff()
    {
        Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var handler = Factory.Services.GetRequiredService<ICommandHandler<SetZoneHeatingStatusCommand>>();

        await Factory.Services.GetRequiredService<ICommandHandler<DisableHeaterCommand>>()
            .HandleAsync(new DisableHeaterCommand(testLocation.LivingRoomHeaterId1, HeaterDisabledReason.User));
        await handler.HandleAsync(new SetZoneHeatingStatusCommand(testLocation.LivingRoomZoneId, HeatingStatus.Off));

        var heater = await client.GetHeatersDetails(testLocation.LivingRoomHeaterId1, testLocation.Token);
        heater.HeaterState.Should().Be(HeaterState.Disabled);
    }
}





