using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HeatKeeper.Server.CQRS;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server.Measurements
{
    public class ReportTemperatureCommandHandler : ICommandHandler<ReportTemperatureCommand[]>
    {
        private readonly IInfluxClient influxClient;
        private readonly IMapper mapper;

        public ReportTemperatureCommandHandler(IInfluxClient influxClient, IMapper mapper)
        {
            this.influxClient = influxClient;
            this.mapper = mapper;
        }

        public async Task HandleAsync(ReportTemperatureCommand[] commands, CancellationToken cancellationToken = default(CancellationToken))
        {
            var influxMeasurements = mapper.Map<ReportTemperatureCommand[], InFluxMeasurement[]>(commands);
            await influxClient.WriteAsync("heatkeeper", influxMeasurements);
        }
    }

    public class InFluxMeasurement
{
   [InfluxTimestamp]
   public DateTime Timestamp { get; set; }

   [InfluxTag( "location" )]
   public string Location { get; set; }

   [InfluxTag( "zone" )]
   public string Zone { get; set; }

   [InfluxField( "temperature" )]
   public double Temperature { get; set; }

   [InfluxField( "humidity" )]
   public long Humidity { get; set; }
}
}