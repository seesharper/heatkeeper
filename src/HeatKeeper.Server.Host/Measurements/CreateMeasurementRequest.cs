namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementRequest
    {
        public CreateMeasurementRequest(string sensorId, double temperature, double humidity)
        {
            SensorId = sensorId;
            Temperature = temperature;
            Humidity = humidity;
        }

        public string SensorId { get; }
        public double Temperature { get; }
        public double Humidity { get; }
    }
}