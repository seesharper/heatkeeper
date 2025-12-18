#nullable enable
using System.Collections.Generic;
using HeatKeeper.Server.Events;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// End-to-end test demonstrating property resolution in action bindings.
/// This simulates what happens when a trigger processes an event with property placeholders.
/// </summary>
public class PropertyResolutionEndToEndTests
{
    [Fact]
    public void TriggerEngine_ResolveParameters_WithPropertyPlaceholders()
    {
        // This test demonstrates the exact flow that happens in TriggerEngine.StartAsync
        // when processing a trigger with property placeholders

        // Arrange - Create a domain event
        var sensorEvent = new TemperatureReadingPayload(
            ZoneId: 42,
            Temperature: 28.5,
            SensorName: "Kitchen Sensor"
        );

        // Create an event envelope (what EventBus produces)
        var eventEnvelope = new EventEnvelope(
            Payload: sensorEvent,
            EventId: 1,
            EventType: "TemperatureReadingPayload",
            OccurredAt: System.DateTimeOffset.UtcNow
        );

        // Create an action binding with property placeholders
        var actionBinding = new ActionBinding(
            ActionId: 2,
            ParameterMap: new Dictionary<string, string>
            {
                ["ZoneId"] = "{{ZoneId}}",  // Will resolve to int 42
                ["Message"] = "Temperature {{Temperature}} in zone {{ZoneId}} from sensor {{SensorName}}"  // Will resolve to string
            }
        );

        // Act - Resolve parameters (this is what TriggerEngine.ResolveParameters does)
        var resolvedParams = new Dictionary<string, object?>();
        foreach (var kvp in actionBinding.ParameterMap)
        {
            resolvedParams[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, eventEnvelope.Payload);
        }

        // Assert - Verify resolution worked correctly
        Assert.Equal(42, resolvedParams["ZoneId"]);  // Single placeholder resolves to typed value

        var message = resolvedParams["Message"] as string;
        Assert.NotNull(message);
        Assert.Contains("Temperature", message);
        Assert.Contains("28", message);  // Temperature value
        Assert.Contains("zone 42", message);
        Assert.Contains("Kitchen Sensor", message);
    }

    [Fact]
    public void TriggerEngine_ResolveParameters_WithLiteralValues()
    {
        // Demonstrate that literals still work (backward compatibility)

        // Arrange
        var sensorEvent = new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0);
        var eventEnvelope = new EventEnvelope(sensorEvent, 1, "TemperatureReadingPayload", System.DateTimeOffset.UtcNow);

        var actionBinding = new ActionBinding(
            ActionId: 1,
            ParameterMap: new Dictionary<string, string>
            {
                ["HeaterId"] = "5",  // Literal value, no placeholder
                ["Reason"] = "Manual override"  // Literal string
            }
        );

        // Act
        var resolvedParams = new Dictionary<string, object?>();
        foreach (var kvp in actionBinding.ParameterMap)
        {
            resolvedParams[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, eventEnvelope.Payload);
        }

        // Assert - Literals pass through unchanged
        Assert.Equal("5", resolvedParams["HeaterId"]);
        Assert.Equal("Manual override", resolvedParams["Reason"]);
    }

    [Fact]
    public void TriggerEngine_ResolveParameters_MixedPlaceholdersAndLiterals()
    {
        // Real-world scenario: Dead sensor triggers notification and heater disable

        // Arrange - Simulate a dead sensor event
        var deadSensorEvent = new DeadSensorEvent(
            ZoneId: 7,
            SensorName: "Bedroom Sensor",
            LastSeen: System.DateTimeOffset.Now.AddHours(-2)
        );

        var eventEnvelope = new EventEnvelope(deadSensorEvent, 998, "DeadSensorEvent", System.DateTimeOffset.UtcNow);

        // Action 1: Disable heaters in the affected zone
        var disableHeatersBinding = new ActionBinding(
            ActionId: 5,
            ParameterMap: new Dictionary<string, string>
            {
                ["ZoneId"] = "{{ZoneId}}"  // Dynamic from event
            }
        );

        // Action 2: Send notification with details
        var notificationBinding = new ActionBinding(
            ActionId: 3,
            ParameterMap: new Dictionary<string, string>
            {
                ["Message"] = "ALERT: Sensor {{SensorName}} in zone {{ZoneId}} is dead!",
                ["Priority"] = "High"  // Literal value
            }
        );

        // Act - Resolve both actions
        var disableParams = new Dictionary<string, object?>();
        foreach (var kvp in disableHeatersBinding.ParameterMap)
        {
            disableParams[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, eventEnvelope.Payload);
        }

        var notifyParams = new Dictionary<string, object?>();
        foreach (var kvp in notificationBinding.ParameterMap)
        {
            notifyParams[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, eventEnvelope.Payload);
        }

        // Assert - Verify both actions got correct parameters
        Assert.Equal(7, disableParams["ZoneId"]);

        var message = notifyParams["Message"] as string;
        Assert.Contains("Bedroom Sensor", message);
        Assert.Contains("zone 7", message);
        Assert.Contains("dead", message);
        Assert.Equal("High", notifyParams["Priority"]);  // Literal unchanged
    }

    // Test event for dead sensor scenario
    [DomainEvent(998, "Dead Sensor Event", "Sensor has stopped responding")]
    public record DeadSensorEvent(int ZoneId, string SensorName, System.DateTimeOffset LastSeen);
}
