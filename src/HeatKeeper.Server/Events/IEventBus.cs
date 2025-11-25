using System.Threading.Channels;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Interface for the event bus that handles domain event publishing and consumption.
/// Domain events are identified by the [DomainEvent] attribute on the payload type.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes a domain event to the bus.
    /// The payload type must have a [DomainEvent] attribute.
    /// </summary>
    /// <typeparam name="T">The type of the event payload (must have [DomainEvent] attribute)</typeparam>
    /// <param name="payload">The event payload</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A ValueTask representing the async operation</returns>
    ValueTask PublishAsync<T>(T payload, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Gets the channel reader for consuming events.
    /// </summary>
    ChannelReader<EventEnvelope> Reader { get; }
}