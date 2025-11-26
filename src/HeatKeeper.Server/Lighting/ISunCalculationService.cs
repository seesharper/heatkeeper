using System;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Service for calculating sunrise and sunset times.
/// </summary>
public interface ISunCalculationService
{
    /// <summary>
    /// Calculates sunrise and sunset times for a given date and location.
    /// </summary>
    /// <param name="date">The date to calculate for</param>
    /// <param name="latitude">Latitude in degrees (-90 to 90)</param>
    /// <param name="longitude">Longitude in degrees (-180 to 180)</param>
    /// <returns>Sunrise and sunset times in UTC</returns>
    Task<(DateTime sunrise, DateTime sunset)> GetSunriseSunsetAsync(DateTime date, double latitude, double longitude);
}