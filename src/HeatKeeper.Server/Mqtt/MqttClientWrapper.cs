using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt;

public interface IMqttClientWrapper
{
    void Dispose();
    Task PublishAsync(string topic, string payload = null);
}

public class MqttClientWrapper : IDisposable, IMqttClientWrapper
{
    private readonly IManagedMqttClient _managedMqttClient;

    private readonly ConcurrentDictionary<string, Func<string, Task>> _subscriptionHandlers = new();

    public MqttClientWrapper(IManagedMqttClient managedMqttClient)
    {
        _managedMqttClient = managedMqttClient;
        _managedMqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
    }

    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        if (_subscriptionHandlers.TryGetValue(args.ApplicationMessage.Topic, out var handler))
        {
            await handler(Encoding.ASCII.GetString(args.ApplicationMessage.Payload));
        }
    }

    public async Task PublishAsync(string topic, string payload = null)
        => await _managedMqttClient.EnqueueAsync(topic, payload);

    public void Dispose()
    {
        _managedMqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceived;
    }
}