using System;

namespace HeatKeeper.Server.Measurements
{
    public abstract class LatestZoneMeasurementCommand
    {
        public long ZoneId { get; set; }

        public MeasurementType MeasurementType { get; set; }

        public double Value { get; set; }

        public DateTime Updated { get; set; }
    }
}
