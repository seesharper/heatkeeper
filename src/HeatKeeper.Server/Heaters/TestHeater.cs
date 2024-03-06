using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Mqtt;

namespace HeatKeeper.Server.Heaters;

[RequireAdminRole]
public record TestHeaterCommand(string MqttTopic, string Payload);

public class TestHeater(ICommandExecutor commandExecutor) : ICommandHandler<TestHeaterCommand>
{
    public async Task HandleAsync(TestHeaterCommand command, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(command.MqttTopic, command.Payload), cancellationToken);
}
