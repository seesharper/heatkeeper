using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class ZoneByExternalSensorQueryHandler : IQueryHandler<ZoneByExternalSensorQuery, long?>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZoneByExternalSensorQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<long?> HandleAsync(ZoneByExternalSensorQuery query, CancellationToken cancellationToken = default)
        {
            return await dbConnection.ExecuteScalarAsync<long?>(sqlProvider.GetZoneIdByExternalSensorId, query);
        }
    }

    [RequireReporterRole]
    public class ZoneByExternalSensorQuery : IQuery<long?>
    {
        public string ExternalSensorId { get; set; }
    }

}
