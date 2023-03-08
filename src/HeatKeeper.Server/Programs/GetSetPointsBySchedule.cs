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
public record SetPointsByScheduleQuery(long ScheduleId) : IQuery<SetPoint[]>;

public record SetPoint(long Id, double Value, double Hysteresis);

public class GetSetPointsByScheduleQueryHandler : IQueryHandler<SetPointsByScheduleQuery, SetPoint[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetSetPointsByScheduleQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<SetPoint[]> HandleAsync(SetPointsByScheduleQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<SetPoint>(_sqlProvider.GetSetPointsBySchedule, query)).ToArray();
}

