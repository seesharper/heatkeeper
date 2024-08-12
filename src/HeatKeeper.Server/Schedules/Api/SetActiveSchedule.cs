namespace HeatKeeper.Server.Schedules.Api;

[RequireBackgroundRole]
// [Post("api/schedules/{scheduleId}/activate")]
public record SetActiveScheduleCommand(long ScheduleId);

public class SetActiveSchedule(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<SetActiveScheduleCommand>
{
    public async Task HandleAsync(SetActiveScheduleCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.SetActiveSchedule, command);
}