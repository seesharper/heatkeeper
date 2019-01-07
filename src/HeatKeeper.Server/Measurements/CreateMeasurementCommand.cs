namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementCommand
    {
        public CreateMeasurementCommand(string sensorId, double batteryLevel ,double temperature, double humidity)
        {
            SensorId = sensorId;
            BatteryLevel = batteryLevel;
            Temperature = temperature;
            Humidity = humidity;
        }

        public string SensorId { get; }
        public double BatteryLevel { get; }
        public double Temperature { get; }
        public double Humidity { get; }
    }
}