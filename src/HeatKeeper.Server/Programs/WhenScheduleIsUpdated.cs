using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Programs;

public class WhenScheduleIsUpdated : ICommandHandler<UpdateScheduleCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICommandHandler<UpdateScheduleCommand> _updateScheduleCommandHandler;

    public WhenScheduleIsUpdated(ICommandExecutor commandExecutor, ICommandHandler<UpdateScheduleCommand> updateScheduleCommandHandler)
    {
        _commandExecutor = commandExecutor;
        _updateScheduleCommandHandler = updateScheduleCommandHandler;
    }

    public async Task HandleAsync(UpdateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await _updateScheduleCommandHandler.HandleAsync(command, cancellationToken);
        await _commandExecutor.ExecuteAsync(new RemoveScheduleFromJanitorCommand(command.ScheduleId), cancellationToken);
        await _commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(command.ScheduleId, command.Name, command.CronExpression), cancellationToken);
    }
}
