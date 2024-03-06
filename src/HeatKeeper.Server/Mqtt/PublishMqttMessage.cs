using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Mqtt;

[RequireBackgroundRole]
public record PublishMqttMessageCommand(string Topic, string Payload);

public class PublishMqttMessage(IManagedMqttClient managedMqttClient) : ICommandHandler<PublishMqttMessageCommand>
{
    public async Task HandleAsync(PublishMqttMessageCommand command, CancellationToken cancellationToken = default)
        => await managedMqttClient.EnqueueAsync(command.Topic, command.Payload);
}
