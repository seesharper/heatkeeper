using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DbReader;
using HeatKeeper.Server.Database;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Zones
{
    public class ZonesByLocationQueryHandler : IQueryHandler<ZonesByLocationQuery, Zone[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZonesByLocationQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<Zone[]> HandleAsync(ZonesByLocationQuery query, CancellationToken cancellationToken = default)
        {
            var result = await dbConnection.ReadAsync<Zone>(sqlProvider.ZonesByLocation, query);
            return result.ToArray();
        }
    }

    [RequireUserRole]
    public class ZonesByLocationQuery : IQuery<Zone[]>
    {
        public long LocationId { get; set; }
    }
}
