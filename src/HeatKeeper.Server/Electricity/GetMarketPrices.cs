using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Electricity;

[RequireBackgroundRole]
public record GetMarketPricesQuery(DateTime DateTime, string Area) : IQuery<MarketPrice[]>;

public record MarketPrice(decimal PricePerKiloWattHour, decimal PricePerKiloWattHourInEuro, DateTime StartDateTime, decimal ExchangeRate, string Area);

public class GetMarketPricesQueryHandler : IQueryHandler<GetMarketPricesQuery, MarketPrice[]>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GetMarketPricesQueryHandler(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<MarketPrice[]> HandleAsync(GetMarketPricesQuery query, CancellationToken cancellationToken = default)
    {
        // GET https://www.hvakosterstrommen.no/api/v1/prices/[ÅR]/[MÅNED]-[DAG]_[PRISOMRÅDE].json
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://www.hvakosterstrommen.no/api/v1/prices/");
        string requestUri = $"{query.DateTime.Year}/{query.DateTime.Month.ToString("D2")}-{query.DateTime.Day.ToString("D2")}_{query.Area}.json";
        var response = await client.GetAsync(requestUri);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Array.Empty<MarketPrice>();
        }
        var result = await response.Content.ReadFromJsonAsync<MarketPriceResult[]>();
        return result.Select(mpr => new MarketPrice(mpr.PricePerKiloWattHour, mpr.PricePerKiloWattHourInEuro, mpr.StartDateTime, mpr.ExchangeRate, query.Area)).ToArray();
    }

    private record MarketPriceResult([property: JsonPropertyName("NOK_per_kWh")] decimal PricePerKiloWattHour, [property: JsonPropertyName("EUR_per_kWh")] decimal PricePerKiloWattHourInEuro, [property: JsonPropertyName("time_start")] DateTime StartDateTime, [property: JsonPropertyName("EXR")] decimal ExchangeRate);
}

