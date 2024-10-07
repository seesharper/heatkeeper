using HeatKeeper.Server.Programs;

namespace HeatKeeper.Server.Schedules;

[RequireBackgroundRole]
public record AddAllSchedulesToJanitorCommand();

public class AddAllSchedulesToJanitorCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<AddAllSchedulesToJanitorCommand>
{
    public async Task HandleAsync(AddAllSchedulesToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        GetAllSchedules.Result[] allSchedules = await queryExecutor.ExecuteAsync(new GetAllSchedulesQuery(), cancellationToken);
        foreach (GetAllSchedules.Result schedule in allSchedules)
        {
            await commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(schedule.Id, schedule.Name, schedule.CronExpression), cancellationToken);
        }
    }
}