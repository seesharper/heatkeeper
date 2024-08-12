namespace HeatKeeper.Server.Programs;

[RequireUserRole]
[Get("api/programs/{ProgramId}/schedules")]
public record SchedulesQuery(long ProgramId) : IQuery<ScheduleInfo[]>;

public record ScheduleInfo(long Id, string Name, string CronExpression);

public class GetSchedules(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SchedulesQuery, ScheduleInfo[]>
{
    public async Task<ScheduleInfo[]> HandleAsync(SchedulesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ScheduleInfo>(sqlProvider.GetSchedulesByProgram, query)).ToArray();
}

