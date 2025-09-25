using System.Threading.Channels;

namespace HeatKeeper.Server.Events;

/// <summary>
/// A lightweight event bus backed by System.Threading.Channels for publishing and consuming strongly-typed domain events.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly Channel<IDomainEvent> _channel;

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
        _channel = Channel.CreateBounded<IDomainEvent>(options);
    }

    /// <summary>
    /// Publishes a strongly-typed domain event to the bus.
    /// </summary>
    /// <typeparam name="T">The type of the event payload</typeparam>
    /// <param name="evt">The event to publish</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A ValueTask representing the async operation</returns>
    public ValueTask PublishAsync<T>(DomainEvent<T> evt, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(evt, ct);

    /// <summary>
    /// Gets the channel reader for consuming events.
    /// </summary>
    public ChannelReader<IDomainEvent> Reader => _channel.Reader;
}