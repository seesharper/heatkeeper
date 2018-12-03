using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using DbReader;
using HeatKeeper.Server.Database;
using System.Linq;

namespace HeatKeeper.Server.Zones
{
    public class GetAllZonesQueryHandler : IQueryHandler<GetAllZonesQuery, ZoneQueryResult[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetAllZonesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<ZoneQueryResult[]> HandleAsync(GetAllZonesQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await dbConnection.ReadAsync<ZoneQueryResult>(sqlProvider.GetAllZones);
            return result.ToArray();
        }
    }
}