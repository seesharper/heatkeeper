using HeatKeeper.Server.Schedules;

namespace HeatKeeper.Server.Programs.Api;

[RequireUserRole]
[Post("/api/programs/{ProgramId}/schedules")]
public record CreateScheduleCommand(long ProgramId, string Name, string CronExpression) : PostCommand, IScheduleCommand;

public class CreateSchedule(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateScheduleCommand>
{
    public async Task HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertSchedule, command);
}