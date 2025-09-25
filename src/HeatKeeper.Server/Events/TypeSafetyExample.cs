namespace HeatKeeper.Server.Events;

/// <summary>
/// Example demonstrating the benefits of strongly-typed domain events.
/// </summary>
public static class TypeSafetyExample
{
    public static void DemonstrateTypeSafety()
    {
        // ✅ Strongly-typed events provide compile-time safety and IntelliSense
        var temperatureEvent = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(
                ZoneId: 1,
                Temperature: 23.5,
                SensorName: "Kitchen-Sensor"
            ));

        // ✅ Type-safe property access with IntelliSense
        var zoneId = temperatureEvent.Payload.ZoneId;          // int - compile-time safe!
        var temperature = temperatureEvent.Payload.Temperature; // double - compile-time safe!
        var sensorName = temperatureEvent.Payload.SensorName;   // string? - compile-time safe!

        // ✅ Motion event with different payload structure
        var motionEvent = DomainEvent<MotionDetectedPayload>.Create(
            new MotionDetectedPayload(
                ZoneId: 2,
                Location: "Living Room",
                DetectedAt: DateTimeOffset.Now
            ));

        // ✅ Door event demonstrating flexibility
        var doorEvent = DomainEvent<DoorEventPayload>.Create(
            new DoorEventPayload(
                DoorId: "main-entrance",
                Action: "opened",
                UserId: "user-123"
            ));

        Console.WriteLine("Strongly-typed events created successfully!");
        Console.WriteLine($"Temperature in zone {zoneId}: {temperature}°C from {sensorName}");
        Console.WriteLine($"Motion detected in: {motionEvent.Payload.Location}");
        Console.WriteLine($"Door {doorEvent.Payload.DoorId} was {doorEvent.Payload.Action}");

        // ✅ Event types are automatically derived from payload type names
        Console.WriteLine($"Event types: {temperatureEvent.EventType}, {motionEvent.EventType}, {doorEvent.EventType}");

        // ✅ Demonstrate compile-time error prevention
        // var badAccess = temperatureEvent.Payload.InvalidProperty; // ❌ Won't compile!
        // var wrongType = (string)temperatureEvent.Payload.ZoneId;  // ❌ Won't compile!
    }
}