using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace HeatKeeper.Server.ExchangeRates;


public class NorwegianBankClient(HttpClient httpClient)
{
    public async Task<decimal> GetExchangeRate(string currency)
    {
        //https://data.norges-bank.no/api/data/EXR/B.EUR.NOK.SP?format=sdmx-json&lastNObservations=1&locale=no

        var response = await httpClient.GetAsync($"EXR/B.{currency}.NOK.SP?format=sdmx-json&lastNObservations=1&locale=no");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(content);
        var value = document.RootElement.GetProperty("data").GetProperty("dataSets").EnumerateArray().First().GetProperty("series").EnumerateObject().First().Value.GetProperty("observations").GetProperty("0").EnumerateArray().First().GetString();
        return decimal.Parse(value, CultureInfo.InvariantCulture);
    }
}