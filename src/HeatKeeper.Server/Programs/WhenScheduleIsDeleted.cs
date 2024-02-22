using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Programs;

public class WhenScheduleIsDeleted : ICommandHandler<DeleteScheduleCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICommandHandler<DeleteScheduleCommand> _deleteScheduleCommandHandler;

    public WhenScheduleIsDeleted(ICommandExecutor commandExecutor, ICommandHandler<DeleteScheduleCommand> deleteScheduleCommandHandler)
    {
        _commandExecutor = commandExecutor;
        _deleteScheduleCommandHandler = deleteScheduleCommandHandler;
    }

    public async Task HandleAsync(DeleteScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await _deleteScheduleCommandHandler.HandleAsync(command, cancellationToken);
        await _commandExecutor.ExecuteAsync(new RemoveScheduleFromJanitorCommand(command.ScheduleId), cancellationToken);
    }
}