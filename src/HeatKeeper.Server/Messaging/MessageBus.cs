using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Messaging;

public interface IMessageBus
{
    void Publish<T>(T message);
    void Subscribe<T>(Delegate handler);
    Task Start();
}

public class MessageBus(IServiceProvider serviceProvider, ILogger<MessageBus> logger) : IMessageBus
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Delegate>> _handlers = new();
    private readonly ConcurrentDictionary<Type, object> _channels = new();

    public void Publish<T>(T message)
    {
        if (_channels.TryGetValue(typeof(T), out var channel))
        {
            ((Channel<T>)channel).Writer.WriteAsync(message);
        }
    }

    public void Subscribe<T>(Delegate handler)
    {
        _handlers.AddOrUpdate(typeof(T), [handler], (type, handlers) =>
        {
            handlers.Add(handler);
            return handlers;
        });
        _channels.AddOrUpdate(typeof(T), Channel.CreateUnbounded<T>(), (type, channel) => channel);
    }

    public async Task Start()
    {
        List<Task> tasks = new();

        foreach (var kvp in _channels)
        {
            var type = kvp.Key;
            var channel = kvp.Value;
            var handlers = _handlers[type];

            var result = (Task)typeof(MessageBus).GetMethod(nameof(ConsumeMessages), BindingFlags.NonPublic | BindingFlags.Instance)
            .MakeGenericMethod(type)
            .Invoke(this, [channel]);
            tasks.Add(result);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ConsumeMessages<TMessage>(Channel<TMessage> channel)
    {
        await foreach (var message in channel.Reader.ReadAllAsync())
        {
            if (_handlers.TryGetValue(typeof(TMessage), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var parameters = handler.Method.GetParameters();
                        var argumentsValues = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].ParameterType == typeof(TMessage))
                            {
                                argumentsValues[i] = message;
                            }
                            else
                            {
                                argumentsValues[i] = scope.ServiceProvider.GetRequiredService(parameters[i].ParameterType);
                            }
                        }
                        try
                        {
                            handler.DynamicInvoke(argumentsValues);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error invoking handler");
                        }
                    }
                }
            }
        }
    }
}