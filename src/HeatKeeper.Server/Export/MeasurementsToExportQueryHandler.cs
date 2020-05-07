using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Measurements;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server.Export
{
    public class MeasurementsToExportQueryHandler : IQueryHandler<MeasurementsToExportQuery, MeasurementToExport[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public MeasurementsToExportQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<MeasurementToExport[]> HandleAsync(MeasurementsToExportQuery query, CancellationToken cancellationToken = default)
        {
            var reader = dbConnection.ExecuteReader(sqlProvider.GetMeasurementsToExport, query);
            //reader.GetFieldType()

            return (await dbConnection.ReadAsync<MeasurementToExport>(sqlProvider.GetMeasurementsToExport, query)).ToArray();
        }
    }

    [RequireReporterRole]
    public class MeasurementsToExportQuery : IQuery<MeasurementToExport[]>
    {
    }

    public class MeasurementToExport
    {
        public long Id { get; set; }

        public long MeasurementType { get; set; }

        [InfluxTag("MeasurementType")]
        public string MeasurementTypeName { get => Enum.GetName(typeof(MeasurementType), MeasurementType); }

        [InfluxField("Value")]
        public double Value { get; set; }

        [InfluxTimestamp]
        public DateTime Created { get; set; }

        [InfluxTag("Zone")]
        public string Zone { get; set; }

        [InfluxTag("Location")]
        public string Location { get; set; }

    }


}
