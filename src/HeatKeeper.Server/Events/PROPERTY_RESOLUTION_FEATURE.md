# Property Resolution Feature for Event System

## Overview

Implemented dynamic property binding for the event system, allowing action parameters to reference domain event properties using the `{{propertyName}}` placeholder pattern.

## Feature Description

When a trigger executes an action in response to a domain event, the action parameters can now contain placeholders that are resolved against the event payload properties. This enables dynamic, context-aware action execution without hardcoding values.

### Syntax

Use double curly braces to reference event properties:
- `{{PropertyName}}` - Resolves to the actual property value from the event
- Multiple placeholders can be used in a single parameter value
- Properties are resolved case-insensitively

### Examples

#### Example 1: Disable Heater in Dead Sensor's Zone
```csharp
// Event payload
public record DeadSensorPayload(int ZoneId, string SensorName);

// Action binding with dynamic zone resolution
new ActionBinding(
    ActionId: 5,
    ParameterMap: new Dictionary<string, string>
    {
        ["HeaterId"] = "{{ZoneId}}" // Resolves to the ZoneId from the event
    }
)
```

#### Example 2: Send Notification with Event Details
```csharp
// Event payload
public record TemperatureReadingPayload(int ZoneId, double Temperature, string SensorName);

// Action binding with multiple placeholders
new ActionBinding(
    ActionId: 3,
    ParameterMap: new Dictionary<string, string>
    {
        ["Message"] = "Temperature {{Temperature}} in zone {{ZoneId}} from {{SensorName}}"
    }
)
```

## Implementation Details

### Components Added

1. **PropertyResolver** (`PropertyResolver.cs`)
   - Static utility class for resolving property placeholders
   - Handles both single property resolution (returns typed value) and mixed content (returns string)
   - Uses reflection to read property values from event payloads
   - Case-insensitive property name matching

2. **Updated TriggerEngine** (`TriggerEngine.cs`)
   - Modified `ResolveParameters` method to use PropertyResolver
   - Automatically resolves placeholders when executing actions

3. **Updated ActionBinding Documentation** (`ActionBinding.cs`)
   - Enhanced XML documentation with examples
   - Clarified placeholder syntax and behavior

### Resolution Behavior

- **Single Placeholder**: `{{ZoneId}}` → Returns the actual typed value (e.g., `int 42`)
- **Mixed Content**: `Zone {{ZoneId}}` → Returns string with resolved value (`"Zone 42"`)
- **Multiple Placeholders**: `{{Property1}} and {{Property2}}` → Returns string with all values resolved
- **Literal Values**: `"fixed value"` → Returned as-is without modification
- **Unknown Properties**: `{{NonExistent}}` → Returns `null` with console warning

## Tests

### Unit Tests (PropertyResolverTests.cs)
- ✅ No placeholders - returns original value
- ✅ Single placeholder - returns typed value
- ✅ Multiple placeholders - returns resolved string
- ✅ Mixed content with placeholders
- ✅ Case-insensitive property names
- ✅ Non-existent properties return null
- ✅ Empty string handling
- ✅ String, numeric, and DateTimeOffset properties
- ✅ Complex payload types

### Integration Tests (PropertyResolutionSimpleTests.cs)
- ✅ Direct PropertyResolver functionality
- ✅ Parameter resolution workflow verification

All 194 tests pass successfully.

## Usage Example

```csharp
// Define event
[DomainEvent(100, "Dead Sensor", "Sensor stopped responding")]
public record DeadSensorPayload(int ZoneId, string SensorName);

// Create trigger
var trigger = new TriggerDefinition(
    Name: "Disable heaters on dead sensor",
    AppliesToEventType: "DeadSensorPayload",
    Conditions: new List<Condition>(),
    Actions: new List<ActionBinding>
    {
        new(
            ActionId: 5, // DisableHeaterAction
            ParameterMap: new Dictionary<string, string>
            {
                ["HeaterId"] = "{{ZoneId}}" // Dynamically resolved from event
            }
        ),
        new(
            ActionId: 3, // SendNotificationAction
            ParameterMap: new Dictionary<string, string>
            {
                ["Message"] = "Dead sensor {{SensorName}} in zone {{ZoneId}}"
            }
        )
    }
);

// When a DeadSensorPayload(ZoneId: 42, SensorName: "Kitchen") is published:
// - DisableHeaterAction executes with HeaterId = 42
// - SendNotificationAction executes with Message = "Dead sensor Kitchen in zone 42"
```

## Benefits

1. **Dynamic Actions**: Actions adapt to event context without configuration changes
2. **Reduced Duplication**: One trigger definition can handle multiple zones/entities
3. **Type Safety**: Single placeholders preserve original property types
4. **Flexibility**: Mix literal text with dynamic values in messages
5. **Maintainability**: Clearer intent in trigger definitions

## Backward Compatibility

Fully backward compatible - existing triggers without placeholders continue to work as literals.
