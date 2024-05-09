namespace HeatKeeper.Server.Programs;

public interface IScheduleCommand
{
    string Name { get; }

    string CronExpression { get; }
}

[RequireUserRole]
public record CreateScheduleCommand(long ProgramId, string Name, string CronExpression) : IScheduleCommand
{
    public long ScheduleId { get; set; }
}

public class CreateScheduleCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateScheduleCommand>
{
    public async Task HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.InsertSchedule, command);
        command.ScheduleId = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLastInsertedRowId);
    }
}