using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt;

public interface ITasmotaClient
{
    void Dispose();
    Task PublishCommand(string topic, string payload = null);
    Task<string> GetStatus(string topic, int timeOut = 2000);
}

public class TasmotaClient : IDisposable, ITasmotaClient
{
    private const string CommandPrefix = "cmnd/";

    private const string StatusPrefix = "stat/";

    private readonly IManagedMqttClient _managedMqttClient;
    private readonly ILogger<TasmotaClient> _logger;
    private readonly ConcurrentDictionary<string, int> _subscribedTopics = new();

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, TaskCompletionSource<string>>> _completionSources = new();

    public TasmotaClient(IManagedMqttClient managedMqttClient, ILogger<TasmotaClient> logger)
    {
        _managedMqttClient = managedMqttClient;
        _logger = logger;
        _managedMqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
    }

    public async Task<string> GetStatus(string topic, int timeOut = 2000)
    {
        string statusTopic = $"{StatusPrefix}{topic}";
        string commandTopic = $"{CommandPrefix}{topic}";

        await SubscribeToStatusTopic(topic, statusTopic);

        Guid requestId = Guid.NewGuid();
        var tcs = new TaskCompletionSource<string>();
        GetCompletionSourceByStatusTopic(statusTopic).TryAdd(requestId, tcs);

        await _managedMqttClient.EnqueueAsync(commandTopic);

        Task completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeOut));
        GetCompletionSourceByStatusTopic(statusTopic).TryRemove(requestId, out _);

        if (completedTask == tcs.Task)
        {
            return await tcs.Task;
        }
        else
        {
            return null;
        }
    }

    private ConcurrentDictionary<Guid, TaskCompletionSource<string>> GetCompletionSourceByStatusTopic(string statusTopic) => _completionSources.GetOrAdd(statusTopic, t => new());

    private async Task SubscribeToStatusTopic(string topic, string subscriptionTopic)
    {
        if (!_subscribedTopics.ContainsKey(subscriptionTopic))
        {
            await _managedMqttClient.SubscribeAsync(subscriptionTopic);
            _subscribedTopics.TryAdd(topic, 42);
        }
    }

    private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        if (_completionSources.TryGetValue(args.ApplicationMessage.Topic, out ConcurrentDictionary<Guid, TaskCompletionSource<string>> completionSources))
        {
            foreach (TaskCompletionSource<string> completionSource in completionSources.Values)
            {
                completionSource.SetResult(Encoding.ASCII.GetString(args.ApplicationMessage.Payload));
            }
        }
        return Task.CompletedTask;
    }

    public async Task PublishCommand(string topic, string payload = null)
    {
        var commandTopic = $"{CommandPrefix}{topic}";
        _logger.LogInformation("Publishing command to topic {topic} with payload {payload}", commandTopic, payload);
        await _managedMqttClient.EnqueueAsync(commandTopic, "OFF");
    }

    public void Dispose()
        => _managedMqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceived;
}