namespace HeatKeeper.Server.Heaters;

public enum HeaterState
{
    /// <summary>
    /// The heater is idle and not currently active.
    /// </summary>    
    Idle = 0,
    
    /// <summary>
    /// The heater is currently active and providing heat.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The heater is temporarily paused and not providing heat, but can be resumed without re-enabling.
    /// </summary>
    Paused = 2,

    /// <summary>
    /// The heater is disabled and cannot be activated until re-enabled.
    /// </summary>
    Disabled = 3
}
