using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Lighting;
using HeatKeeper.Server.Messaging;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Lighting.Examples;

/// <summary>
/// Example service that demonstrates how to respond to outdoor light state changes.
/// This would typically be used to control actual lighting hardware via MQTT, HTTP, or other protocols.
/// </summary>
public class ExampleLightingHandler
{
    private readonly ILogger<ExampleLightingHandler> _logger;

    public ExampleLightingHandler(ILogger<ExampleLightingHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handler method that responds to outdoor light state changes.
    /// This method would be registered with the message bus to receive events.
    /// </summary>
    /// <param name="lightStateChanged">The light state change event</param>
    public async Task HandleLightStateChange(OutdoorLightStateChanged lightStateChanged)
    {
        _logger.LogInformation("Received outdoor light state change for location '{LocationName}' (ID: {LocationId}): {State} at {Timestamp} - {Reason}",
            lightStateChanged.LocationName, lightStateChanged.LocationId, lightStateChanged.State, lightStateChanged.Timestamp, lightStateChanged.Reason);

        switch (lightStateChanged.State)
        {
            case LightState.On:
                await TurnLightsOn(lightStateChanged.LocationId, lightStateChanged.LocationName);
                break;
            case LightState.Off:
                await TurnLightsOff(lightStateChanged.LocationId, lightStateChanged.LocationName);
                break;
        }
    }

    private async Task TurnLightsOn(long locationId, string locationName)
    {
        // Example: Send MQTT command to turn lights on for specific location
        // await mqttClient.PublishAsync($"home/location/{locationId}/outdoor/lights/set", "ON");

        // Example: Send HTTP request to smart home system
        // await httpClient.PostAsync($"https://api.smarthome.local/lights/location/{locationId}/outdoor/on", null);

        // Example: Control via Tasmota devices for specific location
        // await commandExecutor.ExecuteAsync(new TasmotaCommand($"cmnd/location-{locationId}-outdoor-lights/POWER", "ON"));

        _logger.LogInformation("Outdoor lights turned ON for location '{LocationName}' (ID: {LocationId})",
            locationName, locationId);

        // Simulate async operation
        await Task.Delay(100);
    }

    private async Task TurnLightsOff(long locationId, string locationName)
    {
        // Example: Send MQTT command to turn lights off for specific location
        // await mqttClient.PublishAsync($"home/location/{locationId}/outdoor/lights/set", "OFF");

        // Example: Send HTTP request to smart home system
        // await httpClient.PostAsync($"https://api.smarthome.local/lights/location/{locationId}/outdoor/off", null);

        // Example: Control via Tasmota devices for specific location
        // await commandExecutor.ExecuteAsync(new TasmotaCommand($"cmnd/location-{locationId}-outdoor-lights/POWER", "OFF"));

        _logger.LogInformation("Outdoor lights turned OFF for location '{LocationName}' (ID: {LocationId})",
            locationName, locationId);

        // Simulate async operation
        await Task.Delay(100);
    }
}

/// <summary>
/// Example of how to register the lighting event handler in a background service or bootstrapper.
/// </summary>
public class LightingEventRegistration
{
    public static void RegisterLightingEventHandlers(IMessageBus messageBus, IServiceProvider serviceProvider)
    {
        // Register the handler to respond to outdoor light state changes
        messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged lightEvent, ExampleLightingHandler handler) =>
        {
            await handler.HandleLightStateChange(lightEvent);
        });
    }
}