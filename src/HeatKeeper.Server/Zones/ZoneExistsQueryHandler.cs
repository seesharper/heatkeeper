namespace HeatKeeper.Server.Zones
{
    public class ZoneExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ZoneExistsQuery, bool>
    {
        public async Task<bool> HandleAsync(ZoneExistsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.ZoneExists, query)) == 1;
    }

    [RequireUserRole]
    public record ZoneExistsQuery(long LocationId, string Name, long ZoneId = 0) : IQuery<bool>;



}
