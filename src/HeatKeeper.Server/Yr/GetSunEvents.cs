
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.AspNet;
using HeatKeeper.Server.Caching;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Cms;

namespace HeatKeeper.Server.Yr;

[RequireBackgroundRole]
[MemoryCached<HoursBeforeExpiration>(3)]
public record GetSunEventsQuery(double Latitude, double Longitude, DateOnly Date) : IQuery<SunEvents>;

public class GetSunEventsQueryHandler([FromKeyedServices("YrHttpClient")] HttpClient httpClient) : IQueryHandler<GetSunEventsQuery, SunEvents>
{
    public async Task<SunEvents> HandleAsync(GetSunEventsQuery query, CancellationToken cancellationToken = default)
    {
        var latString = query.Latitude.ToString("F6", CultureInfo.InvariantCulture);
        var lngString = query.Longitude.ToString("F6", CultureInfo.InvariantCulture);

        var url = $"weatherapi/sunrise/3.0/sun?lat={latString}&lon={lngString}&date={query.Date:yyyy-MM-dd}&offset=+00:00";
        var response = await httpClient.SendAndHandleResponse<YrSunResponse>(new HttpRequestMessage(HttpMethod.Get, url), cancellationToken: cancellationToken);

        return new SunEvents(
            SunriseUtc: response.Properties.Sunrise.Time,
            SunsetUtc: response.Properties.Sunset.Time
        );
    }
}

public record SunEvents(DateTime SunriseUtc, DateTime SunsetUtc);

internal record YrSunResponse(
    [property: JsonPropertyName("copyright")] string Copyright,
    [property: JsonPropertyName("licenseURL")] string LicenseUrl,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("geometry")] YrGeometry Geometry,
    [property: JsonPropertyName("when")] YrWhen When,
    [property: JsonPropertyName("properties")] YrSunProperties Properties
);

internal record YrGeometry(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("coordinates")] double[] Coordinates
);

internal record YrWhen(
    [property: JsonPropertyName("interval")] string[] Interval
);

internal record YrSunProperties(
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("sunrise")] YrSunEvent Sunrise,
    [property: JsonPropertyName("sunset")] YrSunEvent Sunset,
    [property: JsonPropertyName("solarnoon")] YrSolarNoon Solarnoon,
    [property: JsonPropertyName("solarmidnight")] YrSolarMidnight Solarmidnight
);

internal record YrSunEvent(
    [property: JsonPropertyName("time")] DateTime Time,
    [property: JsonPropertyName("azimuth")] double Azimuth
);

internal record YrSolarNoon(
    [property: JsonPropertyName("time")] DateTime Time,
    [property: JsonPropertyName("disc_centre_elevation")] double DiscCentreElevation,
    [property: JsonPropertyName("visible")] bool Visible
);

internal record YrSolarMidnight(
    [property: JsonPropertyName("time")] DateTime Time,
    [property: JsonPropertyName("disc_centre_elevation")] double DiscCentreElevation,
    [property: JsonPropertyName("visible")] bool Visible
);