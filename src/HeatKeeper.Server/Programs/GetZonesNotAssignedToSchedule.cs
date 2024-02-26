using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Programs;

[RequireAdminRole]
public record GetZonesNotAssignedToScheduleQuery(long ScheduleId) : IQuery<ZoneInfo[]>;

public class GetZonesNotAssignedToScheduleQueryHandler : IQueryHandler<GetZonesNotAssignedToScheduleQuery, ZoneInfo[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetZonesNotAssignedToScheduleQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ZoneInfo[]> HandleAsync(GetZonesNotAssignedToScheduleQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<ZoneInfo>(_sqlProvider.GetZonesNotAssignedToSchedule, query)).ToArray();
}
