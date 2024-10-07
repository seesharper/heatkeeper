namespace HeatKeeper.Server.SetPoints.Api;

[RequireAdminRole]
[Get("api/setPoints/{SetPointId}")]
public record SetPointDetailsQuery(long SetPointId) : IQuery<SetPointDetails>;

public record SetPointDetails(long Id, double Value, double Hysteresis, string ZoneName, string ScheduleName);

public class GetSetPointDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SetPointDetailsQuery, SetPointDetails>
{
    public async Task<SetPointDetails> HandleAsync(SetPointDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await dbConnection.ReadAsync<SetPointDetails>(sqlProvider.GetSetPointDetails, new { id = query.SetPointId })).Single();
    }
}
