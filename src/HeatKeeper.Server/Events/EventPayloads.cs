namespace HeatKeeper.Server.Events;

/// <summary>
/// Strongly-typed payload for temperature reading events.
/// </summary>
public sealed record TemperatureReadingPayload(
    int ZoneId,
    double Temperature,
    string? SensorName = null
);

/// <summary>
/// Strongly-typed payload for motion detection events.
/// </summary>
public sealed record MotionDetectedPayload(
    int ZoneId,
    string Location,
    DateTimeOffset DetectedAt
);

/// <summary>
/// Strongly-typed payload for door events.
/// </summary>
public sealed record DoorEventPayload(
    string DoorId,
    string Action, // "opened" or "closed"
    string? UserId = null
);