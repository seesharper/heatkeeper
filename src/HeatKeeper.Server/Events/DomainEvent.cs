namespace HeatKeeper.Server.Events;

/// <summary>
/// Wrapper for domain events flowing through the event bus.
/// Domain events are identified by the [DomainEvent] attribute on the payload type.
/// </summary>
public sealed record EventEnvelope(
    object Payload,
    int EventId,
    string EventType,
    DateTimeOffset OccurredAt
);