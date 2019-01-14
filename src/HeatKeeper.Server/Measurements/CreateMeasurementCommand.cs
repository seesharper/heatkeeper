namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementCommand
    {
        public CreateMeasurementCommand(string sensorId, int measurementType, double value)
        {
            SensorId = sensorId;
            MeasurementType = measurementType;
            Value = value;
        }

        public string SensorId { get; }
        public int MeasurementType { get; }
        public double Value { get; }
    }
}