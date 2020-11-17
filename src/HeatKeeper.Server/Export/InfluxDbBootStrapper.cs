using System.Threading.Tasks;
using HeatKeeper.Abstractions;
using HeatKeeper.Server.Measurements;
using Vibrant.InfluxDB.Client;

namespace HeatKeeper.Server.Export
{
    public class InfluxDbBootStrapper : IBootStrapper
    {
        private readonly IInfluxClient influxClient;

        public InfluxDbBootStrapper(IInfluxClient influxClient)
        {
            this.influxClient = influxClient;
        }

        public async Task Execute()
        {
            await influxClient.CreateDatabaseAsync(InfluxDbConstants.DatabaseName);
            await influxClient.CreateRetentionPolicyAsync(InfluxDbConstants.DatabaseName, nameof(RetentionPolicy.Hour), "1h", 1, false);
            await influxClient.CreateRetentionPolicyAsync(InfluxDbConstants.DatabaseName, nameof(RetentionPolicy.Day), "1d", 1, false);
            await influxClient.CreateRetentionPolicyAsync(InfluxDbConstants.DatabaseName, nameof(RetentionPolicy.Week), "1w", 1, false);
        }
    }
}
