// using System;
// using System.Collections.Concurrent;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;
// using MQTTnet.Client;
// using MQTTnet.Extensions.ManagedClient;

// namespace HeatKeeper.Server.Mqtt;

// public interface ITasmotaClient
// {
//     void Dispose();
//     Task PublishCommand(string topic, string payload = null);
// }

// public class TasmotaClient : IDisposable, ITasmotaClient
// {
//     private const string CommandPrefix = "cmnd/";

//     private const string StatusPrefix = "stat/";

//     private readonly IManagedMqttClient _managedMqttClient;
//     private readonly ILogger<TasmotaClient> _logger;
//     private readonly ConcurrentDictionary<string, int> _subscribedTopics = new();

//     private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, TaskCompletionSource<string>>> _completionSources = new();

//     public TasmotaClient(IManagedMqttClient managedMqttClient, ILogger<TasmotaClient> logger)
//     {
//         _managedMqttClient = managedMqttClient;
//         _logger = logger;

//     }

//     private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
//     {
//         if (_completionSources.TryGetValue(args.ApplicationMessage.Topic, out ConcurrentDictionary<Guid, TaskCompletionSource<string>> completionSources))
//         {
//             foreach (TaskCompletionSource<string> completionSource in completionSources.Values)
//             {
//                 completionSource.SetResult(Encoding.ASCII.GetString(args.ApplicationMessage.Payload));
//             }
//         }
//         return Task.CompletedTask;
//     }

//     public async Task PublishCommand(string topic, string payload = null)
//     {
//         _logger.LogInformation("Publishing command to topic {topic} with payload {payload}", topic, payload);
//         await _managedMqttClient.EnqueueAsync(topic, payload);
//     }

//     public void Dispose()
//         => _managedMqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceived;
// }