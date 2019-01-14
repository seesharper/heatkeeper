namespace HeatKeeper.Server.Measurements
{
    public class ReportTemperatureCommand
    {
        public ReportTemperatureCommand(string externalSensorId, double batteryLevel ,double temperature, double humidity)
        {
            ExternalSensorId = externalSensorId;
            BatteryLevel = batteryLevel;
            Temperature = temperature;
            Humidity = humidity;
        }

        public string ExternalSensorId { get; }
        public double BatteryLevel { get; }
        public double Temperature { get; }
        public double Humidity { get; }
    }
}