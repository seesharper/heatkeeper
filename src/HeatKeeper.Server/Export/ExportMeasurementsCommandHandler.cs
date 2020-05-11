using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server.Export
{
    public class ExportMeasurementsCommandHandler : ICommandHandler<ExportMeasurementsCommand>
    {
        private readonly IInfluxClient influxClient;

        public ExportMeasurementsCommandHandler(IInfluxClient influxClient)
        {
            this.influxClient = influxClient;
        }

        public async Task HandleAsync(ExportMeasurementsCommand command, CancellationToken cancellationToken = default)
        {
            await influxClient.CreateDatabaseAsync("HeatKeeper");
            await influxClient.WriteAsync("HeatKeeper", "HeatKeeperMeasurements", command.MeasurementsToExport);
        }
    }

    [RequireReporterRole]
    public class ExportMeasurementsCommand
    {
        public MeasurementToExport[] MeasurementsToExport { get; set; }
    }


    public class ComputerInfo
    {
        [InfluxTimestamp]
        public DateTime Timestamp { get; set; }

        [InfluxTag("host")]
        public string Host { get; set; }

        [InfluxTag("region")]
        public string Region { get; set; }

        [InfluxField("cpu")]
        public double CPU { get; set; }

        [InfluxField("ram")]
        public long RAM { get; set; }
    }
}
