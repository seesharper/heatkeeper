namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementCommand
    {
        public CreateMeasurementCommand(long sensorId, int measurementType, double value)
        {
            SensorId = sensorId;
            MeasurementType = measurementType;
            Value = value;
        }

        public long SensorId { get; }
        public int MeasurementType { get; }
        public double Value { get; }
    }
}