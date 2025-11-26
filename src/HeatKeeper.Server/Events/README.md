# Event System - Attribute-Based Domain Events

A clean, modern event bus implementation using attribute-based domain events for maximum simplicity and type safety.

## 🎯 Key Benefits

- **100% Type Safe**: All event payloads are strongly-typed at compile time
- **Attribute-Driven**: Events identified by `[DomainEvent]` attribute
- **Simple API**: Publish payloads directly, no wrapper classes needed
- **IntelliSense Support**: Full IDE support with property suggestions and validation
- **Self-Documenting**: Event structure is clear from payload types
- **Refactoring Safe**: Renaming properties updates all references automatically
- **Performance**: Direct property access without dictionary lookups
- **Clean Architecture**: Minimal boilerplate code

## 🏗️ Architecture

### Core Components

- **`EventEnvelope`**: Internal wrapper containing payload, event type, and timestamp
- **`[DomainEvent]` Attribute**: Marks payload types as domain events
- **`EventBus`**: Channel-based event publishing/consuming
- **`TriggerEngine`**: Rule-based event processing
- **`ActionCatalog`**: Registry of executable actions

## 📝 Usage Examples

### 1. Define Payload Types with Attributes

```csharp
[DomainEvent(Id = 1, Name = "TemperatureReadingPayload", Description = "Temperature reading from a sensor")]
public sealed record TemperatureReadingPayload(
    int ZoneId,
    double Temperature,
    string? SensorName = null
);

[DomainEvent(Id = 2, Name = "MotionDetectedPayload", Description = "Motion detected in a zone")]
public sealed record MotionDetectedPayload(
    int ZoneId,
    string Location,
    DateTimeOffset DetectedAt
);
```

### 2. Publish Events Directly

```csharp
var bus = new EventBus();

// Create payload and publish directly - no wrapper needed!
var payload = new TemperatureReadingPayload(
    ZoneId: 1,
    Temperature: 23.5,
    SensorName: "Kitchen-Sensor"
);

// EventBus automatically wraps in EventEnvelope internally
await bus.PublishAsync(payload);
```

### 3. Type-Safe Property Access

```csharp
// ✅ Compile-time safe property access on payloads
var zoneId = payload.ZoneId;          // int
var temperature = payload.Temperature; // double
var sensorName = payload.SensorName;   // string?

// ❌ These won't compile (type safety!)
// var invalid = payload.InvalidProperty;
// var wrong = (string)payload.ZoneId;
```

### 4. Configure Triggers

Triggers work seamlessly with domain events using reflection-based property access. The event type matches the [DomainEvent] attribute name:

```csharp
var trigger = new TriggerDefinition(
    Name: "Turn heaters off when too warm",
    AppliesToEventType: "TemperatureReadingPayload", // Must match [DomainEvent] Name
    Conditions: new List<Condition>
    {
        new(
            PropertyName: "Temperature",                    // Property name from payload (case-insensitive)
            Operator: ComparisonOperator.GreaterThan,
            Value: "19.5")                                  // Literal threshold value
    },
    Actions: new List<ActionBinding>
    {
        new(
            ActionId: 2,                      // Action ID from [Action] attribute
            ParameterMap: new Dictionary<string, string>
            {
                ["ZoneId"] = "1",                 // Literal zone id
                ["Reason"] = "Temperature too high" // Literal string
            })
    }
);
```

## 🔧 Project Structure

```
eventbus/
├── Program.cs                   # Entry point
├── Demo.cs                      # Demo application
├── DomainEvent.cs               # Core event types
├── EventBus.cs                  # Event bus implementation
├── TriggerEngine.cs             # Trigger processing engine
├── TriggerDefinition.cs         # Trigger model
├── Condition.cs                 # Condition model
├── ActionBinding.cs             # Action binding model
├── ComparisonOperator.cs        # Comparison operators
├── IAction.cs                   # Action interface
├── ActionCatalog.cs             # Action registry
├── ActionParameter.cs           # Action parameter metadata
├── TriggerStore.cs              # JSON persistence
├── EventPayloads.cs             # Sample payload types
├── TurnHeatersOffAction.cs      # Sample action
├── SendNotificationAction.cs    # Sample action
├── TypeSafetyExample.cs         # Usage examples
└── EvolutionExample.cs          # Before/after comparison
```

## 🚀 Getting Started

1. **Define your payload types with [DomainEvent] attribute**:
   ```csharp
   [DomainEvent(Id = 10, Name = "MyEventPayload", Description = "My custom event")]
   public sealed record MyEventPayload(string Id, int Value);
   ```

2. **Publish events directly**:
   ```csharp
   var payload = new MyEventPayload("test-id", 42);
   await bus.PublishAsync(payload);
   // EventBus reads [DomainEvent] attribute and wraps in EventEnvelope
   ```

3. **Configure triggers** to react to events automatically

4. **Implement actions** for business logic execution

The system provides a clean, type-safe foundation for event-driven architectures with minimal boilerplate!