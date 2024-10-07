using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Programs;

[RequireAdminRole]
[Get("api/schedules/{scheduleId}/zones")]
public record GetZonesNotAssignedToScheduleQuery(long ScheduleId) : IQuery<ZoneInfo[]>;

public class GetZonesNotAssignedToScheduleQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetZonesNotAssignedToScheduleQuery, ZoneInfo[]>
{
    public async Task<ZoneInfo[]> HandleAsync(GetZonesNotAssignedToScheduleQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ZoneInfo>(sqlProvider.GetZonesNotAssignedToSchedule, query)).ToArray();
}
