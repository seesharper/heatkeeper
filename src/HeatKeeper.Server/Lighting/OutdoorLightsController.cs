using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Events;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Represents the lighting state for a specific location.
/// </summary>
public class LocationLightingState
{
    public long LocationId { get; set; }
    public string LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public LightState? LastPublishedState { get; set; }
    public DateTime? LastCalculatedDate { get; set; }
    public DateTime TodaySunrise { get; set; }
    public DateTime TodaySunset { get; set; }

    public LocationLightingState(long locationId, string locationName, double latitude, double longitude)
    {
        LocationId = locationId;
        LocationName = locationName;
        Latitude = latitude;
        Longitude = longitude;
    }
}

/// <summary>
/// Service that monitors sunrise/sunset times and publishes outdoor light state changes.
/// Runs as a background service and checks periodically for state changes.
/// </summary>
public interface IOutdoorLightsController
{
    /// <summary>
    /// Gets the current light states for all locations without triggering events.
    /// </summary>
    Task<Dictionary<long, LightState>> GetCurrentLightStatesAsync();

    /// <summary>
    /// Gets the current light state for a specific location without triggering an event.
    /// </summary>
    Task<LightState?> GetCurrentLightStateAsync(long locationId);

    /// <summary>
    /// Manually triggers a light state check and publishes events for all locations if needed.
    /// </summary>
    Task CheckAndPublishLightStates();
}

public class OutdoorLightsController : IOutdoorLightsController
{
    private readonly OutdoorLightsOptions _options;
    private readonly IMessageBus _messageBus;

    private readonly IEventBus _eventBus;
    private readonly TimeProvider _timeProvider;
    private readonly ISunCalculationService _sunCalculationService;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ILogger<OutdoorLightsController> _logger;

    private readonly Dictionary<long, LocationLightingState> _locationStates = new();
    private bool _coordinatesInitialized = false;

    public OutdoorLightsController(
        IConfiguration configuration,
        IMessageBus messageBus,
        IEventBus eventBus,
        TimeProvider timeProvider,
        ISunCalculationService sunCalculationService,
        IQueryExecutor queryExecutor,
        ILogger<OutdoorLightsController> logger)
    {
        _options = configuration.GetOutdoorLightsOptions();
        _messageBus = messageBus;
        _eventBus = eventBus;
        _timeProvider = timeProvider;
        _sunCalculationService = sunCalculationService;
        _queryExecutor = queryExecutor;
        _logger = logger;
    }

    public async Task<Dictionary<long, LightState>> GetCurrentLightStatesAsync()
    {
        await EnsureLocationsInitializedAsync();

        var states = new Dictionary<long, LightState>();
        var now = _timeProvider.GetUtcNow().DateTime;

        foreach (var locationState in _locationStates.Values)
        {
            await EnsureTodaysSunTimesCalculatedAsync(locationState, now);
            states[locationState.LocationId] = GetLightStateForLocation(locationState, now);
        }

        return states;
    }

    public async Task<LightState?> GetCurrentLightStateAsync(long locationId)
    {
        await EnsureLocationsInitializedAsync();

        if (!_locationStates.TryGetValue(locationId, out var locationState))
        {
            return null;
        }

        var now = _timeProvider.GetUtcNow().DateTime;
        await EnsureTodaysSunTimesCalculatedAsync(locationState, now);

        return GetLightStateForLocation(locationState, now);
    }

    public async Task CheckAndPublishLightStates()
    {
        await EnsureLocationsInitializedAsync();

        var now = _timeProvider.GetUtcNow().DateTime;

        foreach (var locationState in _locationStates.Values)
        {
            await EnsureTodaysSunTimesCalculatedAsync(locationState, now);
            var currentState = GetLightStateForLocation(locationState, now);

            // Always publish if we haven't published before, or if state has changed
            if (locationState.LastPublishedState != currentState)
            {
                var reason = locationState.LastPublishedState.HasValue
                    ? $"Light state changed from {locationState.LastPublishedState} to {currentState}"
                    : $"Initial light state: {currentState}";

                var lightEvent = new OutdoorLightStateChanged(
                    locationState.LocationId,
                    locationState.LocationName,
                    currentState,
                    now,
                    reason);

                _logger.LogInformation("Publishing outdoor light state change for location '{LocationName}': {State} - {Reason}",
                    locationState.LocationName, currentState, reason);

                await _messageBus.Publish(lightEvent);
                if (currentState == LightState.On)
                {
                    await _eventBus.PublishAsync(DomainEvent<SunrisePayload>.Create(new SunrisePayload(locationState.LocationName)), CancellationToken.None);
                }    
                else
                {
                    await _eventBus.PublishAsync(DomainEvent<SunsetPayload>.Create(new SunsetPayload(locationState.LocationName)), CancellationToken.None);
                }

                locationState.LastPublishedState = currentState;
            }
            else
            {
                // Publish periodically even if state hasn't changed (as requested for reliability)
                _logger.LogDebug("Outdoor light state unchanged for location '{LocationName}': {State}",
                    locationState.LocationName, currentState);

                var lightEvent = new OutdoorLightStateChanged(
                    locationState.LocationId,
                    locationState.LocationName,
                    currentState,
                    now,
                    $"Periodic state confirmation: {currentState}");
                await _messageBus.Publish(lightEvent);
            }
        }
    }

    private LightState GetLightStateForLocation(LocationLightingState locationState, DateTime now)
    {
        // Defensive check: ensure sunrise/sunset times are properly initialized
        if (locationState.TodaySunrise == DateTime.MinValue || locationState.TodaySunset == DateTime.MinValue)
        {
            _logger.LogError("Sun times not properly initialized for location '{LocationName}' (ID: {LocationId}). Sunrise: {Sunrise}, Sunset: {Sunset}",
                locationState.LocationName, locationState.LocationId, locationState.TodaySunrise, locationState.TodaySunset);
            return LightState.Off; // Fail safe
        }

        try
        {
            var adjustedSunrise = locationState.TodaySunrise.Add(_options.SunriseOffset);
            var adjustedSunset = locationState.TodaySunset.Add(_options.SunsetOffset);

            // Lights should be ON when it's after sunset OR before sunrise
            if (now >= adjustedSunset || now < adjustedSunrise)
            {
                return LightState.On;
            }

            // Lights should be OFF between sunrise and sunset
            return LightState.Off;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogError(ex, "DateTime arithmetic overflow for location '{LocationName}' (ID: {LocationId}). Sunrise: {Sunrise}, Sunset: {Sunset}, SunriseOffset: {SunriseOffset}, SunsetOffset: {SunsetOffset}",
                locationState.LocationName, locationState.LocationId, locationState.TodaySunrise, locationState.TodaySunset, _options.SunriseOffset, _options.SunsetOffset);
            return LightState.Off; // Fail safe
        }
    }

    private async Task EnsureLocationsInitializedAsync()
    {
        if (_coordinatesInitialized) return;

        try
        {
            var locationCoordinates = await _queryExecutor.ExecuteAsync(new GetLocationCoordinatesQuery());

            if (locationCoordinates.Length > 0)
            {
                foreach (var location in locationCoordinates.Where(l => l.Latitude.HasValue && l.Longitude.HasValue))
                {
                    var locationState = new LocationLightingState(
                        location.Id,
                        location.Name,
                        location.Latitude!.Value,   // This is latitude
                        location.Longitude!.Value); // This is longitude

                    _locationStates[location.Id] = locationState;
                }

                _logger.LogInformation(
                    "Outdoor lights controller initialized with {LocationCount} database locations with check interval {CheckInterval}",
                    _locationStates.Count, _options.CheckInterval);
            }
            else
            {
                // Fall back to configuration values - create a default location
                var defaultLocationState = new LocationLightingState(
                    -1, // Use -1 for config-based location
                    "Configuration Default",
                    _options.Latitude,
                    _options.Longitude);

                _locationStates[-1] = defaultLocationState;

                _logger.LogInformation(
                    "No location coordinates found in database, using configuration defaults ({Latitude}, {Longitude}) with check interval {CheckInterval}",
                    _options.Latitude, _options.Longitude, _options.CheckInterval);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to retrieve location coordinates from database, using configuration defaults ({Latitude}, {Longitude})",
                _options.Latitude, _options.Longitude);

            // Fall back to configuration values
            var defaultLocationState = new LocationLightingState(
                -1,
                "Configuration Default",
                _options.Latitude,
                _options.Longitude);

            _locationStates[-1] = defaultLocationState;
        }

        _coordinatesInitialized = true;
    }

    private async Task EnsureTodaysSunTimesCalculatedAsync(LocationLightingState locationState, DateTime now)
    {
        var today = now.Date;

        if (locationState.LastCalculatedDate != today)
        {
            var (sunrise, sunset) = await _sunCalculationService.GetSunriseSunsetAsync(
                today,
                locationState.Latitude,
                locationState.Longitude);

            locationState.TodaySunrise = sunrise;
            locationState.TodaySunset = sunset;
            locationState.LastCalculatedDate = today;

            _logger.LogDebug("Calculated sun times for location '{LocationName}' on {Date}: Sunrise {Sunrise} UTC, Sunset {Sunset} UTC",
                locationState.LocationName, today, sunrise, sunset);

            if (_options.SunriseOffset != TimeSpan.Zero || _options.SunsetOffset != TimeSpan.Zero)
            {
                _logger.LogDebug("Applied offsets for location '{LocationName}': Sunrise offset {SunriseOffset}, Sunset offset {SunsetOffset}",
                    locationState.LocationName, _options.SunriseOffset, _options.SunsetOffset);
            }
        }
    }
}