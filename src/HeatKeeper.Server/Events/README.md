# TestEven- **Automatic Event Types** - Event types derived from payload type names
- **No String Constants** - Eliminates typos and inconsistencies in event type names
- **Performance** - Direct property access without dictionary lookups
- **Clean Architecture** - No legacy compatibility bloatus - Strongly-Typed Domain Events

A clean, modern event bus implementation using strongly-typed domain events for maximum type safety and developer experience.

## 🎯 Key Benefits

- **100% Type Safe**: All events are strongly-typed at compile time
- **IntelliSense Support**: Full IDE support with property suggestions and validation
- **Self-Documenting**: Event structure is clear from payload types
- **Refactoring Safe**: Renaming properties updates all references automatically
- **Performance**: Direct property access without dictionary lookups
- **Clean Architecture**: No legacy compatibility bloat

## 🏗️ Architecture

### Core Components

- **`DomainEvent<T>`**: Strongly-typed event with payload type `T`
- **`IDomainEvent`**: Base interface for all events
- **`EventBus`**: Channel-based event publishing/consuming
- **`TriggerEngine`**: Rule-based event processing
- **`ActionCatalog`**: Registry of executable actions

## 📝 Usage Examples

### 1. Define Payload Types

```csharp
public sealed record TemperatureReadingPayload(
    int ZoneId,
    double Temperature,
    string? SensorName = null
);

public sealed record MotionDetectedPayload(
    int ZoneId,
    string Location,
    DateTimeOffset DetectedAt
);
```

### 2. Create and Publish Events

```csharp
var bus = new EventBus();

// Create event with automatic timestamp and event type
var event = DomainEvent<TemperatureReadingPayload>.Create(
    new TemperatureReadingPayload(
        ZoneId: 1,
        Temperature: 23.5,
        SensorName: "Kitchen-Sensor"
    ));
// event.EventType is automatically "TemperatureReadingPayload"

// Publish to bus
await bus.PublishAsync(event);
```

### 3. Type-Safe Property Access

```csharp
// ✅ Compile-time safe property access
var zoneId = event.Payload.ZoneId;          // int
var temperature = event.Payload.Temperature; // double
var sensorName = event.Payload.SensorName;   // string?

// ❌ These won't compile (type safety!)
// var invalid = event.Payload.InvalidProperty;
// var wrong = (string)event.Payload.ZoneId;
```

### 4. Configure Triggers

Triggers work seamlessly with strongly-typed events using reflection-based property access. The event type is automatically derived from the payload type name:

```csharp
var trigger = new TriggerDefinition(
    Name: "Turn heaters off when too warm",
    AppliesToEventType: "TemperatureReadingPayload", // Payload type name
    Conditions: new List<Condition>
    {
        new(
            LeftSource: "payload",      // Access event payload
            LeftKey: "Temperature",     // Property name (case-insensitive)
            Operator: ComparisonOperator.GreaterThan,
            RightSource: "literal",     // Use literal value
            RightKeyOrLiteral: "19.5")  // Literal threshold value
    },
    Actions: new List<ActionBinding>
    {
        new("TurnHeatersOff", new Dictionary<string, string>
        {
            ["ZoneId"] = "{{payload.ZoneId}}",     // Template resolution from payload
            ["Reason"] = "Temperature too high"     // Literal string
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

1. **Define your payload types**:
   ```csharp
   public sealed record MyEventPayload(string Id, int Value);
   ```

2. **Create and publish events**:
   ```csharp
   var event = DomainEvent<MyEventPayload>.Create(
       new MyEventPayload("test-id", 42));
   // event.EventType is automatically "MyEventPayload"
   await bus.PublishAsync(event);
   ```

3. **Configure triggers** to react to events automatically

4. **Implement actions** for business logic execution

The system provides a clean, type-safe foundation for event-driven architectures without any legacy baggage!