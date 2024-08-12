using HeatKeeper.Server.Schedules;

namespace HeatKeeper.Server.Programs;

public class WhenProgramIsDeleted(ICommandHandler<DeleteProgramCommand> handler, ICommandExecutor commandExecutor) : ICommandHandler<DeleteProgramCommand>
{
    public async Task HandleAsync(DeleteProgramCommand command, CancellationToken cancellationToken = default)
    {
        await commandExecutor.ExecuteAsync(new DeleteAllSchedulesCommand(command.ProgramId), cancellationToken);
        await commandExecutor.ExecuteAsync(new ClearActiveProgramCommand(command.ProgramId), cancellationToken);
        await handler.HandleAsync(command, cancellationToken);
    }
}