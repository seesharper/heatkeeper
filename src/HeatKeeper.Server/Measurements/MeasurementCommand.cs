using System;

namespace HeatKeeper.Server.Measurements
{
    [RequireReporterRole]
    public record MeasurementCommand(
        string SensorId,
        MeasurementType MeasurementType,
        RetentionPolicy RetentionPolicy,
        double Value,
        DateTime Created);
}
