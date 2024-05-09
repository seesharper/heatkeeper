namespace HeatKeeper.Server.Programs;

[RequireUserRole]
[Patch("api/schedules/{scheduleId}")]
public record UpdateScheduleCommand(long ScheduleId, string Name, string CronExpression) : IScheduleCommand;

public class UpdateScheduleCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateScheduleCommand>
{
    public async Task HandleAsync(UpdateScheduleCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateSchedule, command);
}