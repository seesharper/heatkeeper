using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Sun calculation service that uses the sunrise-sunset.org API for accurate astronomical data.
/// </summary>
public class ExternalSunCalculationService : ISunCalculationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalSunCalculationService> _logger;

    public ExternalSunCalculationService(HttpClient httpClient, ILogger<ExternalSunCalculationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculates sunrise and sunset times using the sunrise-sunset.org API.
    /// </summary>
    public async Task<(DateTime sunrise, DateTime sunset)> GetSunriseSunsetAsync(DateTime date, double latitude, double longitude)
    {
        try
        {
            var dateString = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            
            // Use InvariantCulture to ensure period as decimal separator (not comma)
            var latString = latitude.ToString("F6", CultureInfo.InvariantCulture);
            var lngString = longitude.ToString("F6", CultureInfo.InvariantCulture);
            var url = $"https://api.sunrise-sunset.org/json?lat={latString}&lng={lngString}&date={dateString}&formatted=0";

            _logger.LogDebug("Requesting sun times from API: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SunriseApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (apiResponse?.Status != "OK")
            {
                throw new InvalidOperationException($"API returned status: {apiResponse?.Status}");
            }

            // Parse the UTC times
            var sunrise = DateTime.Parse(apiResponse.Results.Sunrise).ToUniversalTime();
            var sunset = DateTime.Parse(apiResponse.Results.Sunset).ToUniversalTime();

            _logger.LogInformation("Retrieved sun times for {Date} at {Latitude}, {Longitude}: sunrise {Sunrise}, sunset {Sunset}",
                dateString, latitude, longitude, sunrise, sunset);

            return (sunrise, sunset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sun times from API for {Date} at {Latitude}, {Longitude}",
                date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), latitude, longitude);

            // Simple fallback: assume standard sunrise at 6:00 UTC and sunset at 18:00 UTC
            // This is a reasonable default for outdoor lighting control when API is unavailable
            _logger.LogWarning("Using simple fallback times: sunrise 06:00 UTC, sunset 18:00 UTC");
            var baseDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            var sunrise = baseDate.AddHours(6);
            var sunset = baseDate.AddHours(18);
            return (sunrise, sunset);
        }
    }

    private class SunriseApiResponse
    {
        public SunriseResults Results { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }

    private class SunriseResults
    {
        public string Sunrise { get; set; } = string.Empty;
        public string Sunset { get; set; } = string.Empty;
        public string SolarNoon { get; set; } = string.Empty;
        public int DayLength { get; set; } = 0;  // Changed to int since API returns seconds as number
        public string CivilTwilightBegin { get; set; } = string.Empty;
        public string CivilTwilightEnd { get; set; } = string.Empty;
        public string NauticalTwilightBegin { get; set; } = string.Empty;
        public string NauticalTwilightEnd { get; set; } = string.Empty;
        public string AstronomicalTwilightBegin { get; set; } = string.Empty;
        public string AstronomicalTwilightEnd { get; set; } = string.Empty;
    }
}