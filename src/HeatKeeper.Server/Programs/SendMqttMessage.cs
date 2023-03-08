using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Mqtt;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SendMqttMessageCommand(string Topic, string PayLoad);

public class SetChannelState : ICommandHandler<SendMqttMessageCommand>
{
    private readonly IMqttClientWrapper _mqttClientWrapper;

    public SetChannelState(IMqttClientWrapper mqttClientWrapper)
        => _mqttClientWrapper = mqttClientWrapper;

    public async Task HandleAsync(SendMqttMessageCommand command, CancellationToken cancellationToken = default)
        => await _mqttClientWrapper.PublishAsync(command.Topic, command.PayLoad);
}