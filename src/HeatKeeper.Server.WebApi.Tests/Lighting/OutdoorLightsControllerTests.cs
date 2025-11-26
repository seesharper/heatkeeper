using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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

public class OutdoorLightsControllerTests
{
    private const long TestLocationId = 1;
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly Mock<ILogger<OutdoorLightsController>> _mockLogger;
    private readonly Mock<ISunCalculationService> _mockSunCalculationService;
    private readonly Mock<IQueryExecutor> _mockQueryExecutor;
    private readonly IConfiguration _configuration;

    public OutdoorLightsControllerTests()
    {
        _mockMessageBus = new Mock<IMessageBus>();
        _fakeTimeProvider = new FakeTimeProvider();
        _mockLogger = new Mock<ILogger<OutdoorLightsController>>();
        _mockSunCalculationService = new Mock<ISunCalculationService>();
        _mockQueryExecutor = new Mock<IQueryExecutor>();

        // Setup query executor to return test location for multi-location support
        var testLocation = new LocationCoordinates(1, "Oslo Test Location", 59.9139, 10.7522); // (Id, Name, Latitude, Longitude)

        _mockQueryExecutor
            .Setup(q => q.ExecuteAsync(It.IsAny<GetLocationCoordinatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { testLocation });

        // Create configuration with test values
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139", // Oslo
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522", // Oslo
            ["OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES"] = "30",
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "0",
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "0"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();
    }

    private OutdoorLightsController CreateController()
    {
        return new OutdoorLightsController(
            _configuration,
            _mockMessageBus.Object,
            _fakeTimeProvider,
            _mockSunCalculationService.Object,
            _mockQueryExecutor.Object,
            _mockLogger.Object);
    }

    private void SetupDefaultSunTimes()
    {
        // Setup default sun times for Oslo, summer solstice
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task GetCurrentLightState_ShouldReturnOn_WhenItIsNight()
    {
        // Arrange - Set time to 2:00 AM UTC on summer solstice (should be night in Oslo)
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc));

        // Mock sun times for June 21, 2024 in Oslo
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // Act
        var state = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert
        state.Should().Be(LightState.On);
    }

    [Fact]
    public async Task GetCurrentLightState_ShouldReturnOff_WhenItIsDay()
    {
        // Arrange - Set time to 12:00 PM UTC on summer solstice (should be day in Oslo)
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 12, 0, 0, DateTimeKind.Utc));

        // Mock sun times for June 21, 2024 in Oslo
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // Act
        var state = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert
        state.Should().Be(LightState.Off);
    }

    [Fact]
    public async Task GetCurrentLightState_ShouldReturnOn_AfterSunset()
    {
        // Arrange - Set time to 10:00 PM UTC on summer solstice (after sunset in Oslo)
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 22, 0, 0, DateTimeKind.Utc));

        // Mock sun times for June 21, 2024 in Oslo
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // Act
        var state = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert
        state.Should().Be(LightState.On);
    }

    [Fact]
    public async Task GetCurrentLightState_WithSunsetOffset_ShouldTurnOnEarlier()
    {
        // Arrange - Configure 30 minute early offset
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "59.9139",
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "10.7522",
            ["OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES"] = "30",
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "0",
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "-30" // 30 minutes earlier
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var sunriseTime = new DateTime(2024, 6, 21, 4, 0, 0, DateTimeKind.Utc);
        var sunsetTime = new DateTime(2024, 6, 21, 20, 0, 0, DateTimeKind.Utc);

        var mockSunService = new Mock<ISunCalculationService>();
        mockSunService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((sunriseTime, sunsetTime));

        // Create a test location with the same coordinates as the configuration
        var testLocation = new LocationCoordinates(TestLocationId, "Test Location", 59.9139, 10.7522); // (Id, Name, Latitude, Longitude)
        var mockQueryExecutor = new Mock<IQueryExecutor>();
        mockQueryExecutor
            .Setup(q => q.ExecuteAsync(It.IsAny<GetLocationCoordinatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { testLocation });

        var controller = new OutdoorLightsController(
            config,
            _mockMessageBus.Object,
            _fakeTimeProvider,
            mockSunService.Object,
            mockQueryExecutor.Object,
            _mockLogger.Object);

        // Set time to when lights should be on due to offset (19:30 = 30 min before sunset with -30 offset)
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 19, 30, 0, DateTimeKind.Utc));

        // Act
        var state = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert - With -30 minute sunset offset, effective sunset is 19:30, so lights should be ON at 19:30
        state.Should().Be(LightState.On);
    }

    [Fact]
    public async Task CheckAndPublishLightState_ShouldPublishEvent_WhenStateChanges()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc)); // Night time

        // Mock sun times for June 21, 2024 in Oslo - sunrise at 3:52 AM, sunset at 8:43 PM
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // Act
        await controller.CheckAndPublishLightStates();

        // Assert
        _mockMessageBus.Verify(mb => mb.Publish(It.Is<OutdoorLightStateChanged>(
            e => e.LocationId == TestLocationId &&
                 e.LocationName == "Oslo Test Location" &&
                 e.State == LightState.On &&
                 e.Reason.Contains("Initial light state"))),
            Times.Once);
    }

    [Fact]
    public async Task CheckAndPublishLightState_ShouldPublishEvent_OnStateChange()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc)); // Night time

        // Mock sun times for June 21, 2024 in Oslo - sunrise at 3:52 AM, sunset at 8:43 PM
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // First verify the initial state at 2 AM (should be ON)
        var initialState = await controller.GetCurrentLightStateAsync(TestLocationId);
        initialState.Should().Be(LightState.On, "lights should be on at 2 AM (before 3:52 AM sunrise)");

        // Establish initial state
        await controller.CheckAndPublishLightStates();
        _mockMessageBus.Reset();

        // Change to day time (should be OFF at 12 PM, which is between 3:52 AM sunrise and 8:43 PM sunset)
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 12, 0, 0, DateTimeKind.Utc));

        // Verify the new state at 12 PM (should be OFF)
        var dayState = await controller.GetCurrentLightStateAsync(TestLocationId);
        dayState.Should().Be(LightState.Off, "lights should be off at 12 PM (between 3:52 AM sunrise and 8:43 PM sunset)");

        // Act
        await controller.CheckAndPublishLightStates();

        // Assert
        _mockMessageBus.Verify(mb => mb.Publish(It.Is<OutdoorLightStateChanged>(
            e => e.LocationId == TestLocationId &&
                 e.LocationName == "Oslo Test Location" &&
                 e.State == LightState.Off &&
                 e.Reason.Contains("Light state changed from On to Off"))),
            Times.Once);
    }

    [Fact]
    public async Task CheckAndPublishLightState_ShouldPublishPeriodicConfirmation_WhenStateUnchanged()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc)); // Night time

        // Mock sun times for June 21, 2024 in Oslo - sunrise at 3:52 AM, sunset at 8:43 PM
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // First call establishes initial state
        await controller.CheckAndPublishLightStates();
        _mockMessageBus.Reset();

        // Act - Call again with same state
        await controller.CheckAndPublishLightStates();

        // Assert
        _mockMessageBus.Verify(mb => mb.Publish(It.Is<OutdoorLightStateChanged>(
            e => e.LocationId == TestLocationId &&
                 e.LocationName == "Oslo Test Location" &&
                 e.State == LightState.On &&
                 e.Reason.Contains("Periodic state confirmation"))),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentLightState_ShouldRecalculateSunTimes_WhenDateChanges()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 12, 0, 0, DateTimeKind.Utc)); // Summer

        // Mock sun times for summer
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 6, 21, 3, 52, 0, DateTimeKind.Utc),
                          new DateTime(2024, 6, 21, 20, 43, 0, DateTimeKind.Utc)));

        // Mock sun times for winter  
        _mockSunCalculationService
            .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 12, 21), 59.9139, 10.7522))
            .ReturnsAsync((new DateTime(2024, 12, 21, 8, 47, 0, DateTimeKind.Utc),
                          new DateTime(2024, 12, 21, 14, 12, 0, DateTimeKind.Utc)));

        var controller = CreateController();

        // Act 1 - Summer day
        var summerState = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Move to winter
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 12, 21, 12, 0, 0, DateTimeKind.Utc)); // Winter

        // Act 2 - Winter day (but different sunrise/sunset times)
        var winterState = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert - Both should be off (daytime), but sun calculations should have been recalculated
        summerState.Should().Be(LightState.Off);
        winterState.Should().Be(LightState.Off);

        // Verify both API calls were made
        _mockSunCalculationService.Verify(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 59.9139, 10.7522), Times.Once);
        _mockSunCalculationService.Verify(s => s.GetSunriseSunsetAsync(new DateTime(2024, 12, 21), 59.9139, 10.7522), Times.Once);
    }

    [Theory]
    [InlineData("51.5074", "-0.1278", "2024-06-21T12:00:00Z", LightState.Off)] // London, summer day
    [InlineData("51.5074", "-0.1278", "2024-06-21T02:00:00Z", LightState.On)]  // London, summer night
    [InlineData("40.7128", "-74.0060", "2024-06-21T15:00:00Z", LightState.Off)] // New York, summer day
    [InlineData("40.7128", "-74.0060", "2024-06-21T05:00:00Z", LightState.On)]  // New York, summer night
    public async Task GetCurrentLightState_ShouldWorkForDifferentLocations(
        string latitude, string longitude, string timeString, LightState expectedState)
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = latitude,
            ["OUTDOOR_LIGHTS_LONGITUDE"] = longitude,
            ["OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES"] = "30",
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "0",
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "0"
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        var lat = double.Parse(latitude, CultureInfo.InvariantCulture);
        var lon = double.Parse(longitude, CultureInfo.InvariantCulture);

        // Create a test location with the specific coordinates for this test
        var testLocation = new LocationCoordinates(TestLocationId, $"Test Location {latitude},{longitude}", lat, lon); // (Id, Name, Latitude, Longitude)
        var mockQueryExecutor = new Mock<IQueryExecutor>();
        mockQueryExecutor
            .Setup(q => q.ExecuteAsync(It.IsAny<GetLocationCoordinatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { testLocation });

        var mockSunService = new Mock<ISunCalculationService>();
        // Set up realistic sun times based on the location/date
        if (latitude == "51.5074") // London
        {
            mockSunService
                .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 51.5074, -0.1278))
                .ReturnsAsync((new DateTime(2024, 6, 21, 4, 43, 0, DateTimeKind.Utc),
                              new DateTime(2024, 6, 21, 20, 21, 0, DateTimeKind.Utc)));
        }
        else if (latitude == "40.7128") // New York
        {
            mockSunService
                .Setup(s => s.GetSunriseSunsetAsync(new DateTime(2024, 6, 21), 40.7128, -74.0060))
                .ReturnsAsync((new DateTime(2024, 6, 21, 9, 25, 0, DateTimeKind.Utc),
                              new DateTime(2024, 6, 21, 23, 31, 0, DateTimeKind.Utc)));
        }

        var controller = new OutdoorLightsController(
            config,
            _mockMessageBus.Object,
            _fakeTimeProvider,
            mockSunService.Object,
            mockQueryExecutor.Object,
            _mockLogger.Object);

        _fakeTimeProvider.SetUtcNow(DateTime.Parse(timeString, null, System.Globalization.DateTimeStyles.RoundtripKind));

        // Act
        var state = await controller.GetCurrentLightStateAsync(TestLocationId);

        // Assert
        state.Should().Be(expectedState);
    }
}