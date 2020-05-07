namespace HeatKeeper.Server
{
    public class ApplicationConfiguration
    {
        public string ConnectionString { get; set; }

        public string Secret { get; set; }

        public string InfluxDBUrl { get; set; }
    }
}
