
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Yr;

[RequireBackgroundRole]
public record GetSunEventsQuery(double Latitude, double Longitude, DateTime Date) : IQuery<SunEvents>;

public class GetSunEventsQueryHandler([FromKeyedServices("YrHttpClient")] HttpClient httpClient) : IQueryHandler<GetSunEventsQuery, SunEvents>
{
    public Task<SunEvents> HandleAsync(GetSunEventsQuery query, CancellationToken cancellationToken = default)
    {
        // https://api.met.no/weatherapi/sunrise/3.0/sun?lat=59.933333&lon=10.716667&date=2025-12-18&offset=+01:00
        var response = httpClient.GetAsync($"weatherapi/sunrise/3.0/sun?lat={query.Latitude}&lon={query.Longitude}&date={query.Date:yyyy-MM-dd}&offset=+00:00", cancellationToken);
        
        throw new NotImplementedException();
    }
}

public record SunEvents(DateTime SunriseUtc, DateTime SunsetUtc);