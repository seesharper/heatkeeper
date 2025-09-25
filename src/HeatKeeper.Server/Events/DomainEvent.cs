namespace HeatKeeper.Server.Events;

/// <summary>
/// Base interface for all domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The type of event (e.g. "TemperatureReading").
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets the payload as an object for reflection-based access.
    /// </summary>
    object? GetPayload();
}

/// <summary>
/// Represents a strongly-typed domain event with a specific payload type.
/// The event type is automatically derived from the payload type name.
/// </summary>
/// <typeparam name="T">The type of the payload data</typeparam>
/// <param name="Payload">Strongly-typed event data</param>
/// <param name="OccurredAt">When the event occurred</param>
public sealed record DomainEvent<T>(
    T Payload,
    DateTimeOffset OccurredAt
) : IDomainEvent
{
    /// <summary>
    /// The event type, automatically derived from the payload type name.
    /// </summary>
    public string EventType { get; } = typeof(T).Name;

    /// <summary>
    /// Gets the payload as an object for reflection-based access.
    /// </summary>
    /// <returns>The payload as an object</returns>
    public object? GetPayload() => Payload;

    /// <summary>
    /// Creates a new strongly-typed domain event with the current timestamp.
    /// The event type is automatically derived from the payload type name.
    /// </summary>
    /// <param name="payload">The event payload</param>
    /// <returns>A new domain event</returns>
    public static DomainEvent<T> Create(T payload)
        => new(payload, DateTimeOffset.UtcNow);

    /// <summary>
    /// Creates a new strongly-typed domain event with a specific timestamp.
    /// The event type is automatically derived from the payload type name.
    /// </summary>
    /// <param name="payload">The event payload</param>
    /// <param name="occurredAt">When the event occurred</param>
    /// <returns>A new domain event</returns>
    public static DomainEvent<T> Create(T payload, DateTimeOffset occurredAt)
        => new(payload, occurredAt);
}