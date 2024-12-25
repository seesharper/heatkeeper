using System.Net.Http;
using System.Xml.Serialization;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.EnergyPrices;

public class EntsoeClient(HttpClient httpClient, IConfiguration configuration, ILogger<EntsoeClient> logger)
{
    public async Task<MarketPrice[]> GetMarketDocument(DateTime date, string area)
    {
        var token = configuration.GetEntsoeApiKey();
        var httpRequest = new HttpRequestBuilder()
            .WithMethod(HttpMethod.Get)
            .AddAcceptHeader("application/xml")
            .AddQueryParameter("documentType", "A44")
            .AddQueryParameter("out_Domain", area)
            .AddQueryParameter("in_Domain", area)
            .AddQueryParameter("securityToken", token)
            .AddQueryParameter("periodStart", DateOnly.FromDateTime(date).ToDateTime(new TimeOnly(0, 0, 0)).ToString("yyyyMMddHHmm"))
            .AddQueryParameter("periodEnd", DateOnly.FromDateTime(date).ToDateTime(new TimeOnly(0, 0, 0)).ToString("yyyyMMddHHmm"))
            .Build();

        var response = await httpClient.SendAndHandleRequest(httpRequest);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to get market document from ENTSOE. Status code: {StatusCode}", response.StatusCode);
            return Array.Empty<MarketPrice>();
        }

        var result = await response.Content.ReadAsStreamAsync();
        XmlSerializer serializer = new(typeof(Publication_MarketDocument));
        var marketDocument = (Publication_MarketDocument)serializer.Deserialize(result);
        return marketDocument.TimeSeries.First().Period.First().Point.Select(p => new MarketPrice(p.priceamount, int.Parse(p.position))).ToArray();
    }
}

public record MarketPrice(decimal Price, int Position);