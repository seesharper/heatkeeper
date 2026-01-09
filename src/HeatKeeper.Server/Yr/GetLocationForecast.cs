using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using CQRS.AspNet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace HeatKeeper.Server.Yr;

public record GetLocationForecastQuery([property: JsonPropertyName("lat")] double Latitude, [property: JsonPropertyName("lon")] double Longitude) : IQuery<LocationForecastCompactResponse>;

public class GetLocationForecastQueryHandler([FromKeyedServices("YrHttpClient")] HttpClient httpClient, IMemoryCache memoryCache) : IQueryHandler<GetLocationForecastQuery, LocationForecastCompactResponse>
{
    public async Task<LocationForecastCompactResponse> HandleAsync(GetLocationForecastQuery query, CancellationToken cancellationToken = default)
    {
        return await memoryCache.GetOrCreateAsync(
            query,
            async entry =>
            {
                var uri = $"weatherapi/locationforecast/2.0/complete?lat={query.Latitude.ToString("F6", CultureInfo.InvariantCulture)}&lon={query.Longitude.ToString("F6", CultureInfo.InvariantCulture)}";
                var response = await httpClient.SendAndHandleResponse(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken: cancellationToken);
                entry.AbsoluteExpiration = response.Content.Headers.Expires.Value.UtcDateTime;
                return await response.Content.As<LocationForecastCompactResponse>(null, cancellationToken);
            });
    }
}

// Root GeoJSON feature returned by /weatherapi/locationforecast/2.0/compact
public sealed record LocationForecastCompactResponse
{
    [JsonPropertyName("type")]
    public string Type { get; init; } // typically "Feature"

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; init; }

    [JsonPropertyName("properties")]
    public ForecastProperties Properties { get; init; }

    // Forward-compat: if the API adds new top-level fields, you won't break
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record Geometry
{
    [JsonPropertyName("type")]
    public string Type { get; init; } // typically "Point"

    // In the docs this is [lon, lat, altitude]
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; init; } // length 2 or 3

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record ForecastProperties
{
    [JsonPropertyName("meta")]
    public ForecastMeta Meta { get; init; }

    [JsonPropertyName("timeseries")]
    public List<TimeSeriesItem> TimeSeries { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record ForecastMeta
{
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    // Units is a dictionary like { "air_temperature": "celsius", ... }
    [JsonPropertyName("units")]
    public Dictionary<string, string> Units { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record TimeSeriesItem
{
    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; init; }

    [JsonPropertyName("data")]
    public TimeSeriesData Data { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record TimeSeriesData
{
    [JsonPropertyName("instant")]
    public Instant Instant { get; init; }

    [JsonPropertyName("next_1_hours")]
    public NextHours Next1Hours { get; init; }

    [JsonPropertyName("next_6_hours")]
    public NextHours Next6Hours { get; init; }

    [JsonPropertyName("next_12_hours")]
    public NextHours Next12Hours { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record Instant
{
    [JsonPropertyName("details")]
    public Details Details { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record NextHours
{
    [JsonPropertyName("summary")]
    public Summary Summary { get; init; }

    [JsonPropertyName("details")]
    public Details Details { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

public sealed record Summary
{
    [JsonPropertyName("symbol_code")]
    public string SymbolCode { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}

/// <summary>
/// Forecast parameter bags:
/// - instant.details contains “state at time instant” parameters
/// - next_X_hours.details contains “period” parameters
///
/// The compact endpoint guarantees only a core set, but the safest approach is:
/// - model common ones as properties (nice DX)
/// - keep ExtensionData for anything else (future-proof)
/// </summary>
public sealed record Details
{
    // Common "instant" values (examples in docs)
    [JsonPropertyName("air_temperature")]
    public double? AirTemperature { get; init; }

    [JsonPropertyName("air_pressure_at_sea_level")]
    public double? AirPressureAtSeaLevel { get; init; }

    [JsonPropertyName("cloud_area_fraction")]
    public double? CloudAreaFraction { get; init; }

    [JsonPropertyName("relative_humidity")]
    public double? RelativeHumidity { get; init; }

    [JsonPropertyName("wind_from_direction")]
    public double? WindFromDirection { get; init; }

    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; init; }

    [JsonPropertyName("wind_speed_of_gust")]
    public double? WindSpeedOfGust { get; init; }

    // Common "next_*_hours" values (examples in docs)
    [JsonPropertyName("precipitation_amount")]
    public double PrecipitationAmount { get; init; }

    [JsonPropertyName("precipitation_amount_min")]
    public double? PrecipitationAmountMin { get; init; }

    [JsonPropertyName("precipitation_amount_max")]
    public double? PrecipitationAmountMax { get; init; }

    [JsonPropertyName("probability_of_precipitation")]
    public double? ProbabilityOfPrecipitation { get; init; }

    [JsonPropertyName("probability_of_thunder")]
    public double? ProbabilityOfThunder { get; init; }

    [JsonPropertyName("air_temperature_min")]
    public double? AirTemperatureMin { get; init; }

    [JsonPropertyName("air_temperature_max")]
    public double? AirTemperatureMax { get; init; }

    // Catch-all for any parameters you didn’t explicitly model
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; }
}
