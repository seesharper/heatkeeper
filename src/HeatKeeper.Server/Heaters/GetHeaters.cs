namespace HeatKeeper.Server.Heaters;

[RequireUserRole]
public record HeatersQuery(long ZoneId) : IQuery<HeaterInfo[]>;

public record HeaterInfo(long Id, string Name);

public class GetHeaters(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<HeatersQuery, HeaterInfo[]>
{
    public async Task<HeaterInfo[]> HandleAsync(HeatersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<HeaterInfo>(sqlProvider.GetHeaters, query)).ToArray();
}