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

public class GetAllSchedules : IQueryHandler<GetAllSchedulesQuery, GetAllSchedules.Result[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetAllSchedules(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<Result[]> HandleAsync(GetAllSchedulesQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<Result>(_sqlProvider.GetAllSchedules)).ToArray();

    public record Result(long Id, long ProgramId, string Name, string CronExpression);
}