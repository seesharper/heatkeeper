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
public record GetScheduleDetailsQuery(long ScheduleId) : IQuery<ScheduleDetails>;

public record ScheduleDetails(long Id, string Name, string CronExpression);

public class GetScheduleDetails : IQueryHandler<GetScheduleDetailsQuery, ScheduleDetails>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetScheduleDetails(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ScheduleDetails> HandleAsync(GetScheduleDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await _dbConnection.ReadAsync<ScheduleDetails>(_sqlProvider.GetScheduleDetails, new { id = query.ScheduleId })).Single();
    }
}