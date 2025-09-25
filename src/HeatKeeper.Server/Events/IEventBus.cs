using System.Threading.Channels;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Interface for the event bus that handles domain event publishing and consumption.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes a strongly-typed domain event to the bus.
    /// </summary>
    /// <typeparam name="T">The type of the event payload</typeparam>
    /// <param name="evt">The event to publish</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A ValueTask representing the async operation</returns>
    ValueTask PublishAsync<T>(DomainEvent<T> evt, CancellationToken ct = default);

    /// <summary>
    /// Gets the channel reader for consuming events.
    /// </summary>
    ChannelReader<IDomainEvent> Reader { get; }
}