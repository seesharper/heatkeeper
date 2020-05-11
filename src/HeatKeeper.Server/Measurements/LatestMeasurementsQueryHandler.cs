using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class LatestTenMeasurementsQueryHandler : IQueryHandler<LatestTenMeasurementsQuery, Measurement[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public LatestTenMeasurementsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<Measurement[]> HandleAsync(LatestTenMeasurementsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ReadAsync<Measurement>(sqlProvider.LatestMeasurements, query)).ToArray();
    }

    [RequireAdminRole]
    public class LatestTenMeasurementsQuery : IQuery<Measurement[]>
    {
        public long Limit { get; set; }
    }

    public class Measurement
    {
        public long Id { get; set; }

        public string ExternalSensorId { get; set; }

        public long MeasurementType { get; set; }

        public string MeasurementTypeName { get => Enum.GetName(typeof(MeasurementType), MeasurementType); }

        public double Value { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Exported { get; set; }
    }


}
