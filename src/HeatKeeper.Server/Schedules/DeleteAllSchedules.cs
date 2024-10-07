
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Schedules.Api;

namespace HeatKeeper.Server.Schedules;

[RequireAdminRole]
public record DeleteAllSchedulesCommand(long ProgramId);

public class DeleteAllSchedules(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<DeleteAllSchedulesCommand>
{
    public async Task HandleAsync(DeleteAllSchedulesCommand command, CancellationToken cancellationToken = default)
    {
        var schedulesToBeDeleted = await queryExecutor.ExecuteAsync(new SchedulesQuery(command.ProgramId), cancellationToken);
        foreach (var schedule in schedulesToBeDeleted)
        {
            await commandExecutor.ExecuteAsync(new DeleteScheduleCommand(schedule.Id), cancellationToken);
        }
    }
}