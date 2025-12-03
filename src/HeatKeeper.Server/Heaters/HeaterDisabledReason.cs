namespace HeatKeeper.Server.Heaters;

/// <summary>
/// Represents the reason why a heater was disabled.
/// </summary>
public enum HeaterDisabledReason
{
    /// <summary>
    /// No specific reason or heater is enabled.
    /// </summary>
    None = 0,

    /// <summary>
    /// Heater was disabled because of a dead sensor.
    /// </summary>
    DeadSensor = 1,

    /// <summary>
    /// Heater was manually disabled by the user.
    /// </summary>
    User = 2,

    /// <summary>
    /// Heater was disabled to prevent overload.
    /// </summary>
    Overload = 3
}
