using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    [RequireReporterRole]
    public record ZoneByExternalSensorQuery(string ExternalSensorId) : IQuery<long?>;

    public class ZoneByExternalSensorQueryHandler : IQueryHandler<ZoneByExternalSensorQuery, long?>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlProvider _sqlProvider;

        public ZoneByExternalSensorQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            _dbConnection = dbConnection;
            _sqlProvider = sqlProvider;
        }

        public async Task<long?> HandleAsync(ZoneByExternalSensorQuery query, CancellationToken cancellationToken = default)
            => await _dbConnection.ExecuteScalarAsync<long?>(_sqlProvider.GetZoneIdByExternalSensorId, query);
    }
}
