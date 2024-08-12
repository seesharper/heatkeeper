namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record SetPointsByScheduleQuery(long ScheduleId) : IQuery<SetPointInfo[]>;

public record SetPointInfo(long Id, string ZoneName, double Value);

public class GetSetPointsByScheduleQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SetPointsByScheduleQuery, SetPointInfo[]>
{
    public async Task<SetPointInfo[]> HandleAsync(SetPointsByScheduleQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SetPointInfo>(sqlProvider.GetSetPointsBySchedule, query)).ToArray();
}

