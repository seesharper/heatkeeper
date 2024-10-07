using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt;

[RequireBackgroundRole]
[Post("api/mqtt")]
public record PublishMqttMessageCommand(string Topic, string Payload);

public class PublishMqttMessage(IManagedMqttClient managedMqttClient) : ICommandHandler<PublishMqttMessageCommand>
{
    public async Task HandleAsync(PublishMqttMessageCommand command, CancellationToken cancellationToken = default)
        => await managedMqttClient.EnqueueAsync(command.Topic, command.Payload);
}
