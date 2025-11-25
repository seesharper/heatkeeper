namespace HeatKeeper.Server.Events;

/// <summary>
/// Demonstrates the evolution to attribute-based event identification.
/// Event types are identified by the [DomainEvent] attribute on payload classes.
/// </summary>
public static class EvolutionExample
{
    public static async Task ShowEvolution()
    {
        var bus = new EventBus();

        Console.WriteLine("=== Current: Attribute-Based Event Identification ===");
        Console.WriteLine("// Events identified by [DomainEvent] attribute on payload class");
        Console.WriteLine();
        Console.WriteLine("// 1. Define payload with attribute:");
        Console.WriteLine("[DomainEvent(Id = 1, Name = \"TemperatureReadingPayload\")]");
        Console.WriteLine("public record TemperatureReadingPayload(int ZoneId, double Temperature);");
        Console.WriteLine();
        Console.WriteLine("// 2. Publish directly - no wrapper needed:");
        Console.WriteLine("var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0);");
        Console.WriteLine("await bus.PublishAsync(payload);");
        Console.WriteLine();

        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0, SensorName: null);
        await bus.PublishAsync(payload);

        Console.WriteLine("// 3. EventBus automatically wraps in EventEnvelope:");
        Console.WriteLine("// - Reads [DomainEvent] attribute");
        Console.WriteLine("// - Sets EventType from attribute name");
        Console.WriteLine("// - Captures OccurredAt timestamp");
        Console.WriteLine();
        Console.WriteLine("// 4. TriggerEngine receives EventEnvelope and processes:");
        Console.WriteLine("var trigger = new TriggerDefinition(");
        Console.WriteLine("    AppliesToEventType: \"TemperatureReadingPayload\", // ✅ Match attribute name");
        Console.WriteLine("    ...);");
        Console.WriteLine();

        Console.WriteLine("=== Benefits ===");
        Console.WriteLine("✅ Simpler API - publish payloads directly");
        Console.WriteLine("✅ No wrapper class needed");
        Console.WriteLine("✅ Event metadata in attributes");
        Console.WriteLine("✅ Consistent with ActionBinding pattern");
        Console.WriteLine("✅ Less boilerplate code");
    }
}