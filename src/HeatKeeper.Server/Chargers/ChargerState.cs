using System.ComponentModel;

namespace HeatKeeper.Server.Chargers;

public enum ChargerState
{
    [Description("The charger is idle and not currently active.")]
    Idle = 0,

    [Description("The charger is currently active and charging.")]
    Active = 1,

    [Description("The charger is temporarily paused.")]
    Paused = 2,

    [Description("The charger is disabled and cannot be activated until re-enabled.")]
    Disabled = 3
}
