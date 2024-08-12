using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record GetAllSchedulesQuery() : IQuery<GetAllSchedules.Result[]>;

public class GetAllSchedules(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetAllSchedulesQuery, GetAllSchedules.Result[]>
{
    public async Task<Result[]> HandleAsync(GetAllSchedulesQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<Result>(sqlProvider.GetAllSchedules)).ToArray();

    public record Result(long Id, long ProgramId, string Name, string CronExpression);
}