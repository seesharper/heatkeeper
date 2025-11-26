namespace HeatKeeper.Server.Events;

/// <summary>
/// Example demonstrating the benefits of strongly-typed domain event payloads.
/// Events are published directly and wrapped internally by the EventBus.
/// </summary>
public static class TypeSafetyExample
{
    public static async Task DemonstrateTypeSafety()
    {
        var bus = new EventBus();

        // ✅ Event payloads provide compile-time safety and IntelliSense
        var temperaturePayload = new TemperatureReadingPayload(
            ZoneId: 1,
            Temperature: 23.5,
            SensorName: "Kitchen-Sensor"
        );

        // ✅ Type-safe property access with IntelliSense
        var zoneId = temperaturePayload.ZoneId;          // int - compile-time safe!
        var temperature = temperaturePayload.Temperature; // double - compile-time safe!
        var sensorName = temperaturePayload.SensorName;   // string? - compile-time safe!

        // ✅ Motion event with different payload structure
        var motionPayload = new MotionDetectedPayload(
            ZoneId: 2,
            Location: "Living Room",
            DetectedAt: DateTimeOffset.Now
        );

        // ✅ Door event demonstrating flexibility
        var doorPayload = new DoorEventPayload(
            DoorId: "main-entrance",
            Action: "opened",
            UserId: "user-123"
        );

        // Publish events - the bus wraps them internally
        await bus.PublishAsync(temperaturePayload);
        await bus.PublishAsync(motionPayload);
        await bus.PublishAsync(doorPayload);

        Console.WriteLine("Strongly-typed event payloads published successfully!");
        Console.WriteLine($"Temperature in zone {zoneId}: {temperature}°C from {sensorName}");
        Console.WriteLine($"Motion detected in: {motionPayload.Location}");
        Console.WriteLine($"Door {doorPayload.DoorId} was {doorPayload.Action}");

        // ✅ Demonstrate compile-time error prevention
        // var badAccess = temperaturePayload.InvalidProperty; // ❌ Won't compile!
        // var wrongType = (string)temperaturePayload.ZoneId;  // ❌ Won't compile!
    }
}