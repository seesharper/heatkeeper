using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using DbReader;
using HeatKeeper.Server.Database;
using System.Linq;

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

        public async Task<ZoneQueryResult[]> HandleAsync(ZonesByLocationQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await dbConnection.ReadAsync<ZoneQueryResult>(sqlProvider.GetAllZones, query);
            return result.ToArray();
        }
    }
}