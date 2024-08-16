namespace HeatKeeper.Server.Schedules.Api;

[RequireBackgroundRole]
[FromParameters]
[Post("api/schedules/{scheduleId}/activate")]
public record SetActiveScheduleCommand(long ScheduleId) : PostCommand;

public class SetActiveSchedule(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<SetActiveScheduleCommand>
{
    public async Task HandleAsync(SetActiveScheduleCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.SetActiveSchedule, command);
}