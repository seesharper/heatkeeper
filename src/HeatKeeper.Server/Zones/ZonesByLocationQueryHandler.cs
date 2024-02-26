using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public class ZonesByLocationQueryHandler : IQueryHandler<ZonesByLocationQuery, ZoneInfo[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZonesByLocationQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<ZoneInfo[]> HandleAsync(ZonesByLocationQuery query, CancellationToken cancellationToken = default)
        {
            var result = await dbConnection.ReadAsync<ZoneInfo>(sqlProvider.ZonesByLocation, query);
            return result.ToArray();
        }
    }

    [RequireUserRole]
    public class ZonesByLocationQuery : IQuery<ZoneInfo[]>
    {
        public long LocationId { get; set; }
    }
}
