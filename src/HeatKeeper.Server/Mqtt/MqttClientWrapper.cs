using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt;

public interface IMqttClientWrapper
{
    void Dispose();
    Task PublishAsync(string topic, string payload = null);
    Task Subscribe(Subscription subscription);
}

public record Subscription(string Topic, Func<string, Task> Handler)
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();
}


public class MqttClientWrapper : IDisposable, IMqttClientWrapper
{
    private readonly IManagedMqttClient _managedMqttClient;

    private ConcurrentDictionary<string, int> _subscribedTopics = new();

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Subscription>> _subscriptionHandlers = new();

    public MqttClientWrapper(IManagedMqttClient managedMqttClient)
    {
        _managedMqttClient = managedMqttClient;
        _managedMqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
    }

    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        if (_subscriptionHandlers.TryGetValue(args.ApplicationMessage.Topic, out ConcurrentDictionary<Guid, Subscription> subscriptions))
        {
            List<Subscription> handledSubscriptions = new();
            foreach (Subscription subscription in subscriptions.Values)
            {
                await subscription.Handler(Encoding.ASCII.GetString(args.ApplicationMessage.Payload));
                handledSubscriptions.Add(subscription);
            }

            foreach (Subscription subscription in handledSubscriptions)
            {
                subscriptions.TryRemove(subscription.SubscriptionId, out _);
            }
        }
    }

    public async Task Subscribe(Subscription subscription)
    {
        _subscriptionHandlers.GetOrAdd(subscription.Topic, t => new()).TryAdd(subscription.SubscriptionId, subscription);
        if (_subscribedTopics.ContainsKey(subscription.Topic))
        {
            await _managedMqttClient.SubscribeAsync(subscription.Topic);
            _subscribedTopics.TryAdd(subscription.Topic, 42);
        }
    }

    public async Task PublishAsync(string topic, string payload = null)
        => await _managedMqttClient.EnqueueAsync(topic, payload);

    public void Dispose()
        => _managedMqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceived;
}