using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Schedules.Api;
using HeatKeeper.Server.SetPoints;

namespace HeatKeeper.Server.Schedules;

public class WhenScheduleIsDeleted(ICommandExecutor commandExecutor, ICommandHandler<DeleteScheduleCommand> deleteScheduleCommandHandler) : ICommandHandler<DeleteScheduleCommand>
{
    public async Task HandleAsync(DeleteScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await commandExecutor.ExecuteAsync(new DeleteAllSetPointsCommand(command.ScheduleId), cancellationToken);
        await commandExecutor.ExecuteAsync(new SetActiveScheduleIdToNullCommand(command.ScheduleId), cancellationToken);
        await deleteScheduleCommandHandler.HandleAsync(command, cancellationToken);
        await commandExecutor.ExecuteAsync(new RemoveScheduleFromJanitorCommand(command.ScheduleId), cancellationToken);
    }
}