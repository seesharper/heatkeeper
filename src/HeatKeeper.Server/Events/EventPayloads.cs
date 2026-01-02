namespace HeatKeeper.Server.Events;

/// <summary>
/// Strongly-typed payload for temperature reading events.
/// </summary>
[DomainEvent(1, "Temperature Reading", "Event triggered when a temperature sensor reports a new reading")]
public sealed record TemperatureReadingPayload(
    int ZoneId,
    double Temperature,
    string? SensorName = null
);

/// <summary>
/// Strongly-typed payload for motion detection events.
/// </summary>
[DomainEvent(2, "Motion Detected", "Event triggered when motion is detected in a zone")]
public sealed record MotionDetectedPayload(
    [property: Lookup("api/locations")] int LocationId,
    [property: Lookup("api/locations/{locationId}/zones")] int ZoneId,
    DateTimeOffset DetectedAt
);

/// <summary>
/// Strongly-typed payload for door events.
/// </summary>
[DomainEvent(3, "Door Event", "Event triggered when a door is opened or closed")]
public sealed record DoorEventPayload(
    string DoorId,
    string Action, // "opened" or "closed"
    string? UserId = null
);
