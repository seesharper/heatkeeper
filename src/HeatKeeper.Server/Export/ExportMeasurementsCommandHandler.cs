using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Measurements;
using Vibrant.InfluxDB.Client;
namespace HeatKeeper.Server.Export
{
    public class ExportMeasurementsCommandHandler : ICommandHandler<ExportMeasurementsCommand>
    {
        private readonly IInfluxClient influxClient;

        public ExportMeasurementsCommandHandler(IInfluxClient influxClient)
            => this.influxClient = influxClient;

        public async Task HandleAsync(ExportMeasurementsCommand command, CancellationToken cancellationToken = default)
        {
            var measurementsGroupedByRetentionPolicy = command.MeasurementsToExport.GroupBy(m => m.RetentionPolicy);

            foreach (var group in measurementsGroupedByRetentionPolicy)
            {
                var writeOptions = GetWriteOptions(group.Key);
                await influxClient.WriteAsync(InfluxDbConstants.DatabaseName, InfluxDbConstants.MeasurementName, group, writeOptions);
            }
        }

        private InfluxWriteOptions GetWriteOptions(RetentionPolicy retentionPolicy)
        {
            return new InfluxWriteOptions()
            {
                RetentionPolicy = retentionPolicy == RetentionPolicy.None ? null : Enum.GetName(typeof(RetentionPolicy), retentionPolicy)
            };
        }
    }

    [RequireReporterRole]
    public class ExportMeasurementsCommand
    {
        public MeasurementToExport[] MeasurementsToExport { get; set; }
    }
}
