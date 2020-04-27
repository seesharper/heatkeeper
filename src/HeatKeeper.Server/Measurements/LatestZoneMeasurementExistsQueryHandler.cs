using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class LatestZoneMeasurementExistsQueryHandler : IQueryHandler<LatestZoneMeasurementExistsQuery, bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public LatestZoneMeasurementExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(LatestZoneMeasurementExistsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.LatestZoneMeasurementExists, query)) == 1;
    }

    [RequireReporterRole]
    public class LatestZoneMeasurementExistsQuery : IQuery<bool>
    {
        public long ZoneId { get; set; }

        public MeasurementType MeasurementType { get; set; }
    }
}
