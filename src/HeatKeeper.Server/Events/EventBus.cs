using System.Reflection;
using System.Threading.Channels;

namespace HeatKeeper.Server.Events;

/// <summary>
/// A lightweight event bus backed by System.Threading.Channels for publishing and consuming domain events.
/// Domain events are identified by the [DomainEvent] attribute on the payload type.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly Channel<EventEnvelope> _channel;

    /// <summary>
    /// Initializes a new instance of the EventBus with optional channel configuration.
    /// </summary>
    /// <param name="options">Channel options. If null, uses default bounded channel with capacity 1024.</param>
    public EventBus(BoundedChannelOptions? options = null)
    {
        options ??= new BoundedChannelOptions(capacity: 1024)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = false
        };
        _channel = Channel.CreateBounded<EventEnvelope>(options);
    }

    /// <summary>
    /// Publishes a domain event to the bus.
    /// The payload type must have a [DomainEvent] attribute.
    /// </summary>
    /// <typeparam name="T">The type of the event payload (must have [DomainEvent] attribute)</typeparam>
    /// <param name="payload">The event payload</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A ValueTask representing the async operation</returns>
    public ValueTask PublishAsync<T>(T payload, CancellationToken ct = default) where T : class
    {
        var attribute = typeof(T).GetCustomAttribute<DomainEventAttribute>()
            ?? throw new InvalidOperationException($"Type {typeof(T).Name} must have a [DomainEvent] attribute");

        var envelope = new EventEnvelope(
            Payload: payload,
            EventType: typeof(T).Name,
            OccurredAt: DateTimeOffset.UtcNow
        );

        return _channel.Writer.WriteAsync(envelope, ct);
    }

    /// <summary>
    /// Gets the channel reader for consuming events.
    /// </summary>
    public ChannelReader<EventEnvelope> Reader => _channel.Reader;
}