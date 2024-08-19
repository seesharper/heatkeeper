namespace HeatKeeper.Server.Schedules.Api;

[RequireUserRole]
[Get("api/schedules/{ScheduleId}/setPoints")]
public record SetPointsQuery(long ScheduleId) : IQuery<SetPointInfo[]>;

public record SetPointInfo(long Id, string ZoneName, double Value);

public class GetSetPoints(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SetPointsQuery, SetPointInfo[]>
{
    public async Task<SetPointInfo[]> HandleAsync(SetPointsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SetPointInfo>(sqlProvider.GetSetPointsBySchedule, query)).ToArray();
}

