using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record SchedulesByProgramQuery(long ProgramId) : IQuery<ScheduleInfo[]>;

public record ScheduleInfo(long Id, string Name, string CronExpression);

public class GetSchedulesByProgramQueryHandler : IQueryHandler<SchedulesByProgramQuery, ScheduleInfo[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetSchedulesByProgramQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ScheduleInfo[]> HandleAsync(SchedulesByProgramQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<ScheduleInfo>(_sqlProvider.GetSchedulesByProgram, query)).ToArray();
}

