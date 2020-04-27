using System;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Measurements
{
    [RequireReporterRole]
    public class MeasurementCommand
    {
        public MeasurementCommand(string sensorId, MeasurementType measurementType, double value, DateTime created)
        {
            SensorId = sensorId;
            MeasurementType = measurementType;
            Value = value;
            Created = created;
        }

        public string SensorId { get; }
        public MeasurementType MeasurementType { get; }
        public double Value { get; }
        public DateTime Created { get; }
    }
}
