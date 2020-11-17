namespace HeatKeeper.Server.Measurements
{

    public enum MeasurementType
    {
        Temperature = 1,
        Humidity = 2,
        BatteryLevel = 3,
        AirPressure = 4,

        /// <summary>
        /// Active power in import direction
        /// </summary>
        ActivePowerImport = 5,

        /// <summary>
        /// Instantaneous current of L1(mA).
        /// </summary>
        CurrentPhase1 = 6,

        /// <summary>
        /// Instantaneous current of L2 (mA).
        /// </summary>
        CurrentPhase2 = 7,

        /// <summary>
        /// Instantaneous current of L3 (mA).
        /// </summary>
        CurrentPhase3 = 8,

        /// <summary>
        /// Instantaneous voltage L1-L2
        /// </summary>
        VoltageBetweenPhase1AndPhase2 = 9,

        /// <summary>
        /// Instantaneous voltage L1-L3
        /// </summary>
        VoltageBetweenPhase1AndPhase3 = 10,

        /// <summary>
        /// Instantaneous voltage L2-L3
        /// </summary>
        VoltageBetweenPhase2AndPhase3 = 11,

        /// <summary>
        /// Cumulativeactive import active energy (Wh)
        /// </summary>
        CumulativePowerImport = 12
    }
}
