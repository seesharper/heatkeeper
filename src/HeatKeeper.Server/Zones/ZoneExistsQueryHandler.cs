using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class ZoneExistsQueryHandler : IQueryHandler<ZoneExistsQuery, bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZoneExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(ZoneExistsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.ZoneExists, query)) == 1;
    }

    [RequireUserRole]
    public record ZoneExistsQuery(long ZoneId, long LocationId, string Name) : IQuery<bool>;
}
