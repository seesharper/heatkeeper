using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MQTTnet.Extensions.ManagedClient;

namespace HeatKeeper.Server.Events;

public record MqttActionParameters(
    [property: Description("The MQTT topic to publish to"), Required] string Topic,
    [property: Description("The payload to send"), Required] string Payload);

[Action(3, "Send MQTT Message", "Sends a message to an MQTT topic")]
public sealed class MqttAction : IAction<MqttActionParameters>
{
    private readonly IManagedMqttClient _managedMqttClient;

    public MqttAction(IManagedMqttClient managedMqttClient)
    {
        _managedMqttClient = managedMqttClient;
    }

    public async Task ExecuteAsync(MqttActionParameters parameters, CancellationToken cancellationToken = default)
    {
        await _managedMqttClient.EnqueueAsync(parameters.Topic, parameters.Payload);
    }
}