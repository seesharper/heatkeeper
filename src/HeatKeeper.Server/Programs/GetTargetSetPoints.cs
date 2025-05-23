namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record GetTargetSetPointsQuery() : IQuery<TargetSetPoint[]>;

public record TargetSetPoint(long ZoneId, double Value, double Hysteresis);

public class GetTargetSetPoints : IQueryHandler<GetTargetSetPointsQuery, TargetSetPoint[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetTargetSetPoints(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<TargetSetPoint[]> HandleAsync(GetTargetSetPointsQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<TargetSetPoint>(_sqlProvider.GetTargetSetPoints)).ToArray();
}