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
    int ZoneId,
    string Location,
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

/// <summary>
/// Strongly-typed payload for sundown events.
/// </summary>
[DomainEvent(4, "Sunrise Event", "Event triggered at sunrise")]
public sealed record SunriseEvent(string Location);

/// <summary>
/// Strongly-typed payload for sunrise events.
/// </summary>
[DomainEvent(5, "Sunset Event", "Event triggered at sunset")]
public sealed record SunsetEvent(string Location);