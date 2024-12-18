using System.Net.Http;
using System.Xml.Serialization;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Http;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.EnergyPrices;

public class EntsoeClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<Publication_MarketDocument> GetMarketDocument(DateTime date, string area)
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
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStreamAsync();
        XmlSerializer serializer = new(typeof(Publication_MarketDocument));
        return (Publication_MarketDocument)serializer.Deserialize(result);
    }
}