# Outdoor Lights Controller

The Outdoor Lights Controller is a system component that automatically switches outdoor lights on at sunset and off at sunrise, with configurable location coordinates and check intervals.

## Overview

The system consists of several components:

1. **OutdoorLightsController** - The main service that calculates sunrise/sunset times and determines light state
2. **OutdoorLightsHostedService** - Background service that runs the controller on a timer
3. **ExternalSunCalculationService** - Service that uses sunrise-sunset.org API for accurate sun calculations
4. **OutdoorLightStateChanged** - Event published when light state changes

## Features

- ✅ Automatic sunrise/sunset calculation based on location coordinates
- ✅ Configurable check interval (default: 30 minutes)
- ✅ Periodic event publishing for reliability (in case messages are missed)
- ✅ Configurable sunrise/sunset offsets for fine-tuning
- ✅ Event-driven architecture using the existing MessageBus system
- ✅ Proper logging for monitoring and debugging

## Configuration

The system can be configured using environment variables with the `HEATKEEPER_` prefix:

| Environment Variable                               | Default   | Description                                  |
| -------------------------------------------------- | --------- | -------------------------------------------- |
| `HEATKEEPER_OUTDOOR_LIGHTS_LATITUDE`               | `59.9139` | Latitude in degrees (-90 to 90)              |
| `HEATKEEPER_OUTDOOR_LIGHTS_LONGITUDE`              | `10.7522` | Longitude in degrees (-180 to 180)           |
| `HEATKEEPER_OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES` | `30`      | How often to check and emit events (minutes) |
| `HEATKEEPER_OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES` | `0`       | Offset for sunrise time (negative = earlier) |
| `HEATKEEPER_OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES`  | `0`       | Offset for sunset time (negative = earlier)  |

### Example Configuration

```bash
# Configure for London coordinates
export HEATKEEPER_OUTDOOR_LIGHTS_LATITUDE=51.5074
export HEATKEEPER_OUTDOOR_LIGHTS_LONGITUDE=-0.1278

# Check every 15 minutes instead of 30
export HEATKEEPER_OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES=15

# Turn lights on 30 minutes before sunset
export HEATKEEPER_OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES=-30

# Turn lights off 30 minutes after sunrise
export HEATKEEPER_OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES=30
```

## Usage

### 1. Subscribing to Light State Events

To respond to outdoor light state changes, subscribe to the `OutdoorLightStateChanged` event using the MessageBus:

```csharp
// In a background service or bootstrapper
messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged lightEvent, IServiceProvider serviceProvider) =>
{
    using var scope = serviceProvider.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<OutdoorLightsEventHandler>>();
    
    logger.LogInformation("Light state changed to: {State} - {Reason}", 
        lightEvent.State, lightEvent.Reason);
        
    switch (lightEvent.State)
    {
        case LightState.On:
            // Turn your outdoor lights on
            await TurnLightsOn();
            break;
        case LightState.Off:
            // Turn your outdoor lights off
            await TurnLightsOff();
            break;
    }
});
```

### 2. Manual Light State Checks

You can also manually check the current light state:

```csharp
// Inject IOutdoorLightsController
var currentState = lightsController.GetCurrentLightState();

// Manually trigger a state check and event publication
await lightsController.CheckAndPublishLightState();
```

### 3. Integration Examples

#### MQTT Integration
```csharp
messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged lightEvent, IMqttClient mqttClient) =>
{
    var payload = lightEvent.State == LightState.On ? "ON" : "OFF";
    await mqttClient.PublishAsync("home/outdoor/lights/set", payload);
});
```

#### HTTP API Integration
```csharp
messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged lightEvent, HttpClient httpClient) =>
{
    var endpoint = lightEvent.State == LightState.On 
        ? "https://api.smarthome.local/lights/outdoor/on" 
        : "https://api.smarthome.local/lights/outdoor/off";
    await httpClient.PostAsync(endpoint, null);
});
```

#### Tasmota Device Control
```csharp
messageBus.Subscribe<OutdoorLightStateChanged>(async (OutdoorLightStateChanged lightEvent, ICommandExecutor commandExecutor) =>
{
    var command = lightEvent.State == LightState.On ? "ON" : "OFF";
    await commandExecutor.ExecuteAsync(new TasmotaCommand("cmnd/outdoor-lights/POWER", command));
});
```

## Event Details

### OutdoorLightStateChanged Event

```csharp
public record OutdoorLightStateChanged(LightState State, DateTime Timestamp, string Reason);
```

- **State**: Either `LightState.On` or `LightState.Off`
- **Timestamp**: When the event was generated (UTC)
- **Reason**: Human-readable reason for the state change

### Example Events

```
Light state changed from Off to On (sunset occurred)
Light state changed from On to Off (sunrise occurred)
Initial light state: On
Periodic state confirmation: Off
```

## System Integration

The lights controller is automatically registered and started with the HeatKeeper application:

1. **Service Registration**: `OutdoorLightsController` is registered as a singleton in `ServerCompositionRoot`
2. **Background Service**: `OutdoorLightsHostedService` is registered in `Program.cs`
3. **Event Bus**: Uses the existing `MessageBus` for event publication
4. **Configuration**: Uses the existing configuration system

## Monitoring and Troubleshooting

The system provides comprehensive logging:

- **Info Level**: State changes, service startup/shutdown
- **Debug Level**: Sunrise/sunset calculations, periodic confirmations
- **Error Level**: Calculation errors, configuration issues

Example log messages:
```
[INFO] Outdoor lights controller initialized for location (59.9139, 10.7522) with check interval 00:30:00
[INFO] Publishing outdoor light state change: On - Light state changed from Off to On
[DEBUG] Calculated sun times for 2024-01-15: Sunrise 08:42:33 UTC, Sunset 15:18:27 UTC
```

## Testing

The system respects the `AppEnvironment.IsRunningFromTests` flag and will not start the background service during test execution.

## Implementation Notes

- Uses a simplified sunrise/sunset calculation algorithm suitable for most locations
- Automatically recalculates sun times daily
- Publishes events both on state changes and periodically for reliability
- Thread-safe and handles timezone considerations by working in UTC
- Follows the existing HeatKeeper architecture patterns