namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record GetAllSchedulesQuery() : IQuery<GetAllSchedules.Result[]>;

public class GetAllSchedules(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetAllSchedulesQuery, GetAllSchedules.Result[]>
{
    public async Task<Result[]> HandleAsync(GetAllSchedulesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<Result>(sqlProvider.GetAllSchedules)).ToArray();

    public record Result(long Id, long ProgramId, string Name, string CronExpression);
}