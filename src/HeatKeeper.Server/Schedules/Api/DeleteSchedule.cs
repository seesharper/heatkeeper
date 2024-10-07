namespace HeatKeeper.Server.Schedules.Api;

[RequireUserRole]
[Delete("api/schedules/{scheduleId}")]
public record DeleteScheduleCommand(long ScheduleId) : DeleteCommand;

public class DeleteScheduleCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteScheduleCommand>
{
    public async Task HandleAsync(DeleteScheduleCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteSchedule, command);
}