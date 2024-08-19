namespace HeatKeeper.Server.Schedules.Api;

[RequireUserRole]
[Get("api/schedules/{ScheduleId}")]
public record ScheduleDetailsQuery(long ScheduleId) : IQuery<ScheduleDetails>;

public record ScheduleDetails(long Id, string Name, string CronExpression);

public class GetScheduleDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ScheduleDetailsQuery, ScheduleDetails>
{
    public async Task<ScheduleDetails> HandleAsync(ScheduleDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ScheduleDetails>(sqlProvider.GetScheduleDetails, new { id = query.ScheduleId })).Single();
}