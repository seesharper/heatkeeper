using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DbReader;
using HeatKeeper.Server.Database;
using System.Linq;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Zones
{
    public class ZonesByLocationQueryHandler : IQueryHandler<ZonesByLocationQuery, ZoneQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZonesByLocationQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<ZoneQueryResult[]> HandleAsync(ZonesByLocationQuery query, CancellationToken cancellationToken = default)
        {
            var result = await dbConnection.ReadAsync<ZoneQueryResult>(sqlProvider.ZonesByLocation, query);
            return result.ToArray();
        }
    }
}
