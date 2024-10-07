using HeatKeeper.Server.Schedules.Api;

namespace HeatKeeper.Server.Schedules;

public class WhenScheduleIsUpdated(ICommandExecutor commandExecutor, ICommandHandler<UpdateScheduleCommand> updateScheduleCommandHandler) : ICommandHandler<UpdateScheduleCommand>
{
    public async Task HandleAsync(UpdateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await updateScheduleCommandHandler.HandleAsync(command, cancellationToken);
        await commandExecutor.ExecuteAsync(new RemoveScheduleFromJanitorCommand(command.ScheduleId), cancellationToken);
        await commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(command.ScheduleId, command.Name, command.CronExpression), cancellationToken);
    }
}
