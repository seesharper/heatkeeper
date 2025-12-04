using System.ComponentModel;

namespace HeatKeeper.Server.Heaters;

/// <summary>
/// Represents the reason why a heater was disabled.
/// </summary>
public enum HeaterDisabledReason
{
    /// <summary>
    /// No specific reason or heater is enabled.
    /// </summary>
    [Description("No specific reason or heater is enabled.")]
    None = 0,

    /// <summary>
    /// Heater was disabled because of a dead sensor.
    /// </summary>
    [Description("Heater was disabled because of a dead sensor.")]
    DeadSensor = 1,

    /// <summary>
    /// Heater was manually disabled by the user.
    /// </summary>
    [Description("Heater was manually disabled by the user.")]
    User = 2,

    /// <summary>
    /// Heater was disabled to prevent overload.
    /// </summary>
    [Description("Heater was disabled to prevent overload.")]
    Overload = 3
}
