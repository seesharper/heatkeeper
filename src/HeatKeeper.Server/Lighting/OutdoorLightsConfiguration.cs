using HeatKeeper.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Configuration options for the outdoor lights controller.
/// </summary>
public class OutdoorLightsOptions
{
    /// <summary>
    /// Latitude of the location in degrees (-90 to 90).
    /// Default: 59.9139 (Oslo, Norway)
    /// </summary>
    public double Latitude { get; set; } = 59.9139;

    /// <summary>
    /// Longitude of the location in degrees (-180 to 180).
    /// Default: 10.7522 (Oslo, Norway)
    /// </summary>
    public double Longitude { get; set; } = 10.7522;

    /// <summary>
    /// How often to check and potentially emit light state events.
    /// Default: 30 minutes
    /// </summary>
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Offset to apply to sunrise time (negative = earlier, positive = later).
    /// This allows fine-tuning when lights turn off relative to actual sunrise.
    /// Default: 0 minutes (use actual sunrise time)
    /// </summary>
    public TimeSpan SunriseOffset { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Offset to apply to sunset time (negative = earlier, positive = later).
    /// This allows fine-tuning when lights turn on relative to actual sunset.
    /// Default: 0 minutes (use actual sunset time)
    /// </summary>
    public TimeSpan SunsetOffset { get; set; } = TimeSpan.Zero;
}

/// <summary>
/// Extension methods for reading outdoor lights configuration.
/// </summary>
public static class OutdoorLightsConfigurationExtensions
{
    public static double GetOutdoorLightsLatitude(this IConfiguration configuration)
        => configuration.GetValue("OUTDOOR_LIGHTS_LATITUDE", 59.9139); // Oslo default

    public static double GetOutdoorLightsLongitude(this IConfiguration configuration)
        => configuration.GetValue("OUTDOOR_LIGHTS_LONGITUDE", 10.7522); // Oslo default

    public static TimeSpan GetOutdoorLightsCheckInterval(this IConfiguration configuration)
        => TimeSpan.FromMinutes(configuration.GetValue("OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES", 30));

    public static TimeSpan GetOutdoorLightsSunriseOffset(this IConfiguration configuration)
        => TimeSpan.FromMinutes(configuration.GetValue("OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES", 0));

    public static TimeSpan GetOutdoorLightsSunsetOffset(this IConfiguration configuration)
        => TimeSpan.FromMinutes(configuration.GetValue("OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES", 0));

    public static OutdoorLightsOptions GetOutdoorLightsOptions(this IConfiguration configuration)
        => new()
        {
            Latitude = configuration.GetOutdoorLightsLatitude(),
            Longitude = configuration.GetOutdoorLightsLongitude(),
            CheckInterval = configuration.GetOutdoorLightsCheckInterval(),
            SunriseOffset = configuration.GetOutdoorLightsSunriseOffset(),
            SunsetOffset = configuration.GetOutdoorLightsSunsetOffset()
        };
}