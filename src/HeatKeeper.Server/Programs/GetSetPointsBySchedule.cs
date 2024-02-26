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
public record SetPointsByScheduleQuery(long ScheduleId) : IQuery<SetPointInfo[]>;

public record SetPointInfo(long Id, string ZoneName, double Value);

public class GetSetPointsByScheduleQueryHandler : IQueryHandler<SetPointsByScheduleQuery, SetPointInfo[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetSetPointsByScheduleQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<SetPointInfo[]> HandleAsync(SetPointsByScheduleQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<SetPointInfo>(_sqlProvider.GetSetPointsBySchedule, query)).ToArray();
}

