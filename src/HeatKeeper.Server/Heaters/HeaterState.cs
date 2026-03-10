using System.ComponentModel;

namespace HeatKeeper.Server.Heaters;

public enum HeaterState
{
    /// <summary>
    /// The heater is idle and not currently active.
    /// </summary>
    [Description("The heater is idle and not currently active.")]
    Idle = 0,

    /// <summary>
    /// The heater is currently active and providing heat.
    /// </summary>
    [Description("The heater is currently active and providing heat.")]
    Active = 1,

    /// <summary>
    /// The heater is temporarily paused and not providing heat, but can be resumed without re-enabling.
    /// </summary>
    [Description("The heater is temporarily paused and not providing heat, but can be resumed without re-enabling.")]
    Paused = 2,

    /// <summary>
    /// The heater is disabled and cannot be activated until re-enabled.
    /// </summary>
    [Description("The heater is disabled and cannot be activated until re-enabled.")]
    Disabled = 3
}
