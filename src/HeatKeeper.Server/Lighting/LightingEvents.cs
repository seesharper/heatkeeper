namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Represents the desired state of outdoor lights.
/// </summary>
public enum LightState
{
    /// <summary>
    /// Lights should be turned off (daytime)
    /// </summary>
    Off,

    /// <summary>
    /// Lights should be turned on (nighttime)
    /// </summary>
    On
}

/// <summary>
/// Event raised when the outdoor light state changes.
/// </summary>
public record OutdoorLightStateChanged(long LocationId, string LocationName, LightState State, DateTime Timestamp, string Reason);