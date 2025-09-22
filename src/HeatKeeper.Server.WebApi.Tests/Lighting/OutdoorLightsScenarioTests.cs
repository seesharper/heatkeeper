using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.Lighting;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

/// <summary>
/// Tests that demonstrate real-world scenarios for the outdoor lights controller.
/// </summary>
public class OutdoorLightsScenarioTests
{
    private const long TestLocationId = 1;

    private static Mock<IQueryExecutor> CreateMockQueryExecutor()
    {
        var mockQueryExecutor = new Mock<IQueryExecutor>();
        var testLocation = new LocationCoordinates(1, "Oslo Test Location", 59.9139, 10.7522); // (Id, Name, Latitude, Longitude)
        mockQueryExecutor
            .Setup(q => q.ExecuteAsync(It.IsAny<GetLocationCoordinatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { testLocation });
        return mockQueryExecutor;
    }

    [Fact]
    public async Task Scenario_NorwayWinterDay_ShouldHaveShortDaylight()
    {
        // Arrange - December 21st in Oslo (winter solstice)
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 12, 21, 10, 0, 0, DateTimeKind.Utc)); // 10 AM UTC

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock winter sun times for Oslo (very short day)
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 12, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 12, 21, 8, 47, 0, DateTimeKind.Utc),
                          new DateTime(2024, 12, 21, 14, 12, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Act & Assert - Should be daytime even though it's a short winter day
        var morningState = await controller.GetCurrentLightStateAsync(TestLocationId);
        morningState.Should().Be(LightState.Off, "10 AM should be daytime even in Norwegian winter");

        // Test evening - should be night by 4 PM
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 12, 21, 16, 0, 0, DateTimeKind.Utc)); // 4 PM UTC
        var eveningState = await controller.GetCurrentLightStateAsync(TestLocationId);
        eveningState.Should().Be(LightState.On, "4 PM should be nighttime in Norwegian winter");
    }

    [Fact]
    public async Task Scenario_NorwaySummerDay_ShouldHaveLongDaylight()
    {
        // Arrange - June 21st in Oslo (summer solstice)
        var fakeTimeProvider = new FakeTimeProvider();

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock summer sun times for Oslo (very long day)
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 1, 50, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 47, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Act & Assert - Very early morning should be daytime
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 4, 0, 0, DateTimeKind.Utc)); // 4 AM UTC
        var earlyMorning = await controller.GetCurrentLightStateAsync(TestLocationId);
        earlyMorning.Should().Be(LightState.Off, "4 AM should be daytime in Norwegian summer");

        // Late evening should still be daytime
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 20, 0, 0, DateTimeKind.Utc)); // 8 PM UTC
        var lateEvening = await controller.GetCurrentLightStateAsync(TestLocationId);
        lateEvening.Should().Be(LightState.Off, "8 PM should still be daytime in Norwegian summer");

        // Very late should be nighttime
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 23, 0, 0, DateTimeKind.Utc)); // 11 PM UTC
        var veryLate = await controller.GetCurrentLightStateAsync(TestLocationId);
        veryLate.Should().Be(LightState.On, "11 PM should be nighttime even in Norwegian summer");
    }

    [Fact]
    public async Task Scenario_TurnLightsOnEarlierForSafety_WithSunsetOffset()
    {
        // Arrange - Configure lights to turn on 30 minutes before sunset
        var fakeTimeProvider = new FakeTimeProvider();
        var events = new List<OutdoorLightStateChanged>();

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "-30" // 30 minutes earlier
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        mockMessageBus.Setup(mb => mb.Publish(It.IsAny<OutdoorLightStateChanged>()))
            .Returns(Task.CompletedTask)
            .Callback<OutdoorLightStateChanged>(e => events.Add(e));

        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock sun times for the test scenario
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Act - Test around the adjusted sunset time
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 21, 10, 0, DateTimeKind.Utc)); // Should be before adjusted sunset
        await controller.CheckAndPublishLightStates();

        // Assert
        events.Should().HaveCount(1);
        events[0].State.Should().Be(LightState.On, "lights should turn on 30 minutes before actual sunset");
        events[0].Reason.Should().Contain("Initial");
    }

    [Fact]
    public async Task Scenario_KeepLightsOnLongerInMorning_WithSunriseOffset()
    {
        // Arrange - Configure lights to turn off 30 minutes after sunrise
        var fakeTimeProvider = new FakeTimeProvider();
        var events = new List<OutdoorLightStateChanged>();

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo  
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "30" // 30 minutes later
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        mockMessageBus.Setup(mb => mb.Publish(It.IsAny<OutdoorLightStateChanged>()))
            .Returns(Task.CompletedTask)
            .Callback<OutdoorLightStateChanged>(e => events.Add(e));

        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock sun times for the test scenario
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Start with lights on (night time)
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc));
        await controller.CheckAndPublishLightStates();

        // Move to time that would normally be after sunrise but before adjusted sunrise
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 3, 15, 0, DateTimeKind.Utc)); // Should still be "night" with offset
        await controller.CheckAndPublishLightStates();

        // Assert
        events.Should().HaveCount(2);
        events[0].State.Should().Be(LightState.On, "initial state should be on at night");
        events[1].State.Should().Be(LightState.On, "lights should stay on 30 minutes after actual sunrise");
        events[1].Reason.Should().Contain("Periodic state confirmation");
    }

    [Theory]
    [InlineData("2024-03-20", 6)] // Spring equinox - approximately 6 AM sunrise at equator
    [InlineData("2024-06-21", 3)] // Summer solstice - early sunrise in Oslo 
    [InlineData("2024-09-23", 6)] // Fall equinox - approximately 6 AM sunrise at equator
    [InlineData("2024-12-21", 8)] // Winter solstice - late sunrise in Oslo
    public async Task Scenario_SeasonalVariations_ShouldReflectDifferentSunriseTimes(string dateString, int expectedSunriseHourApprox)
    {
        // Arrange
        var date = DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        var fakeTimeProvider = new FakeTimeProvider();

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock sun times for different seasons
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(date.Year, date.Month, date.Day, expectedSunriseHourApprox, 0, 0, DateTimeKind.Utc),
                          new DateTime(date.Year, date.Month, date.Day, 20, 0, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Act - Test several hours around expected sunrise
        var testHours = new[] { expectedSunriseHourApprox - 2, expectedSunriseHourApprox, expectedSunriseHourApprox + 2 };
        var states = new List<LightState>();

        foreach (var hour in testHours)
        {
            fakeTimeProvider.SetUtcNow(new DateTime(date.Year, date.Month, date.Day, hour, 0, 0, DateTimeKind.Utc));
            var state = await controller.GetCurrentLightStateAsync(TestLocationId);
            states.Add(state ?? LightState.Off); // Handle null by defaulting to Off
        }

        // Assert - Before sunrise should be ON, after should be OFF
        states[0].Should().Be(LightState.On, $"2 hours before expected sunrise should be night");
        states[2].Should().Be(LightState.Off, $"2 hours after expected sunrise should be day");
    }

    [Fact]
    public async Task Scenario_ReliabilityThroughPeriodicEvents_ShouldSendMultipleEventsForSameState()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var events = new List<OutdoorLightStateChanged>();

        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139",
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var mockMessageBus = new Mock<IMessageBus>();
        mockMessageBus.Setup(mb => mb.Publish(It.IsAny<OutdoorLightStateChanged>()))
            .Returns(Task.CompletedTask)
            .Callback<OutdoorLightStateChanged>(e => events.Add(e));

        var mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        var mockSunService = new Mock<ISunCalculationService>();

        // Mock sun times for the test scenario
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = new OutdoorLightsController(config, mockMessageBus.Object, fakeTimeProvider, mockSunService.Object, CreateMockQueryExecutor().Object, mockLogger.Object);

        // Set to a consistent night time
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc));

        // Act - Call multiple times without changing time (simulating periodic checks)
        await controller.CheckAndPublishLightStates(); // Initial
        await controller.CheckAndPublishLightStates(); // Periodic confirmation
        await controller.CheckAndPublishLightStates(); // Another periodic confirmation

        // Assert
        events.Should().HaveCount(3);
        events[0].Reason.Should().Contain("Initial light state");
        events[1].Reason.Should().Contain("Periodic state confirmation");
        events[2].Reason.Should().Contain("Periodic state confirmation");

        // All should have the same state (ON for night time)
        events.Should().AllSatisfy(e => e.State.Should().Be(LightState.On));
    }
}