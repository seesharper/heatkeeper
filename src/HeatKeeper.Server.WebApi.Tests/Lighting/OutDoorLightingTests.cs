using System;
using System.Linq;
using System.Threading;
using CQRS.AspNet.Testing;
using CQRS.Command.Abstractions;
using CsvHelper;
using HeatKeeper.Server.Lighting;
using HeatKeeper.Server.Locations.Api;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

public class OutDoorLightingTests : TestBase
{

    [Fact]
    public async Task ShouldTurnLightsOnWhenSunsets()
    {
        var fakeTimeProvider = Factory.UseFakeTimeProvider(TestData.Clock.EarlyMorning);
        var testLocation = await Factory.CreateTestLocation();

        var commandExecutor = Factory.Services.GetRequiredService<ICommandExecutor>();
        await commandExecutor.ExecuteAsync(new ScheduleSunriseAndSunsetEventsCommand(DateOnly.FromDateTime(TestData.Clock.Today)), CancellationToken.None);

        var janitor = Factory.Services.GetRequiredService<IJanitor>();
        await janitor.Run("Sunset_Location_" + testLocation.LocationId);

    }
}