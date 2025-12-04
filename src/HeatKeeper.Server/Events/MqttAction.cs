using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HeatKeeper.Server.Authorization;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Events;

[Action(3, "Send MQTT Message", "Sends a message to an MQTT topic")]
[RequireBackgroundRole]
public record MqttCommand(
    [property: Description("The MQTT topic to publish to"), Required] string Topic,
    [property: Description("The payload to send"), Required] string Payload);

public sealed class MqttCommandHandler(IManagedMqttClient managedMqttClient) : ICommandHandler<MqttCommand>
{
    public async Task HandleAsync(MqttCommand command, CancellationToken cancellationToken = default)
    {
        await managedMqttClient.EnqueueAsync(command.Topic, command.Payload);
    }
}