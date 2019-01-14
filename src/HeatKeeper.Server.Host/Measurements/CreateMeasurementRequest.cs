namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementRequest
    {
        public CreateMeasurementRequest(string sensorId, MeasurementType measurementType, double value)
        {
            SensorId = sensorId;
            MeasurementType = measurementType;
            Value = value;
        }

        public string SensorId { get; }
        public MeasurementType MeasurementType { get; }
        public double Value { get; }
    }
}