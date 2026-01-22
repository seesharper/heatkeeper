using System.Runtime.CompilerServices;
using System.Text;
using CQRS.Execution;
using Microsoft.AspNetCore.Http.HttpResults;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt.Api;

[Get("api/mqtt")]
[RequireUserRole]
public record StreamMqttMessagesQuery(string Topic) : IQuery<ServerSentEventsResult<MqttMessage>>;

public record MqttMessage(string Topic, string Payload, DateTime Timestamp);

public class StreamMqttMessages(IManagedMqttClient managedMqttClient) : IQueryHandler<StreamMqttMessagesQuery, ServerSentEventsResult<MqttMessage>>
{
    public async Task<ServerSentEventsResult<MqttMessage>> HandleAsync(StreamMqttMessagesQuery query, CancellationToken cancellationToken = default)
    {
        var messages = GetMessagesAsync(query.Topic, cancellationToken);
        return TypedResults.ServerSentEvents(messages);
    }

    private async IAsyncEnumerable<MqttMessage> GetMessagesAsync(string topic, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Subscribe to the topic
        await managedMqttClient.SubscribeAsync(topic);

        // Create a channel to receive messages
        var channel = System.Threading.Channels.Channel.CreateUnbounded<MqttMessage>();

        // Set up message handler
        var handler = new Func<MqttApplicationMessageReceivedEventArgs, Task>(async args =>
        {

            var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
            var message = new MqttMessage(
                args.ApplicationMessage.Topic,
                payload,
                DateTime.UtcNow
            );
            await channel.Writer.WriteAsync(message, cancellationToken);

        });

        managedMqttClient.ApplicationMessageReceivedAsync += handler;

        try
        {
            // Stream messages until cancellation
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return message;
            }
        }
        finally
        {
            // Clean up: unsubscribe and remove handler
            managedMqttClient.ApplicationMessageReceivedAsync -= handler;
            await managedMqttClient.UnsubscribeAsync(topic);
            channel.Writer.Complete();
        }
    }
}
