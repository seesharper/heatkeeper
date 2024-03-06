using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Export;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Sensors;
using Janitor;
using LightInject;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
}





