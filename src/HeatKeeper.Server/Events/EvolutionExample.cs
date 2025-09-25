namespace HeatKeeper.Server.Events;

/// <summary>
/// Demonstrates the evolution from manual event type strings to automatic type derivation.
/// </summary>
public static class EvolutionExample
{
    public static void ShowEvolution()
    {
        Console.WriteLine("=== Before: Manual Event Type Strings ===");
        Console.WriteLine("// Potential for typos and inconsistencies");
        Console.WriteLine("var evt1 = DomainEvent<TemperatureReadingPayload>.Create(");
        Console.WriteLine("    \"TemperatureReading\",  // ❌ String could be wrong");
        Console.WriteLine("    new TemperatureReadingPayload(...));");
        Console.WriteLine();
        Console.WriteLine("var evt2 = DomainEvent<TemperatureReadingPayload>.Create(");
        Console.WriteLine("    \"TempReading\",        // ❌ Inconsistent naming");
        Console.WriteLine("    new TemperatureReadingPayload(...));");
        Console.WriteLine();
        Console.WriteLine("var trigger = new TriggerDefinition(");
        Console.WriteLine("    AppliesToEventType: \"TemperatureReading\", // ❌ Must match manually");
        Console.WriteLine("    ...);");
        Console.WriteLine();

        Console.WriteLine("=== After: Automatic Type Derivation ===");
        Console.WriteLine("// Guaranteed consistency and type safety");

        var evt = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0));

        Console.WriteLine($"var evt = DomainEvent<TemperatureReadingPayload>.Create(payload);");
        Console.WriteLine($"// evt.EventType = \"{evt.EventType}\" ✅ Automatically correct!");
        Console.WriteLine();
        Console.WriteLine("var trigger = new TriggerDefinition(");
        Console.WriteLine($"    AppliesToEventType: \"{evt.EventType}\", // ✅ Copy from event type");
        Console.WriteLine("    ...);");
        Console.WriteLine();

        Console.WriteLine("=== Benefits ===");
        Console.WriteLine("✅ No typos in event type names");
        Console.WriteLine("✅ Consistent naming automatically");
        Console.WriteLine("✅ Refactoring renames event types too");
        Console.WriteLine("✅ Less code to write");
        Console.WriteLine("✅ Impossible to have mismatched event types");
    }
}