using HeatKeeper.Server.EnergyPriceAreas;
using HeatKeeper.Server.ExchangeRates;

namespace HeatKeeper.Server.EnergyPrices.Api;

[RequireBackgroundRole]
[Post("/api/energy-prices/import")]
public record ImportEnergyPricesCommand(DateTime DateToImport) : PostCommand;

public class ImportEnergyPricesCommandHandler(EntsoeClient entsoeClient, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<ImportEnergyPricesCommand>
{
    private static string[] norwegianPriceAreas = ["10YNO-1--------2", "10YNO-2--------T", "10YNO-3--------J", "10YNO-4--------9", "10Y1001A1001A48H"];

    public async Task HandleAsync(ImportEnergyPricesCommand command, CancellationToken cancellationToken = default)
    {
        var configuredPriceAreas = await queryExecutor.ExecuteAsync(new GetConfiguredPriceAreasQuery(), cancellationToken);
        var exchangeRate = (await queryExecutor.ExecuteAsync(new GetExchangeRateQuery("EUR"), cancellationToken)).Rate;
        var dateToImport = DateOnly.FromDateTime(command.DateToImport).ToDateTime(new TimeOnly(0, 0, 0));

        foreach (var priceArea in configuredPriceAreas)
        {
            var energyPricesExists = await queryExecutor.ExecuteAsync(new EnergyPricesExistsQuery(priceArea.EnergyPriceAreaId, command.DateToImport), cancellationToken);
            if (energyPricesExists)
            {
                continue;
            }
            var markedPrices = await entsoeClient.GetMarketDocument(command.DateToImport, priceArea.EIC_Code);


            var prices = new decimal?[24];
            for (var i = 0; i < markedPrices.Length; i++)
            {
                prices[markedPrices[i].Position - 1] = markedPrices[i].Price;
            }

            for (var i = 0; i < prices.Length; i++)
            {
                if (prices[i] == null)
                {
                    prices[i] = prices[i - 1];
                }
            }

            for (var i = 0; i < 24; i++)
            {
                var startTime = dateToImport.AddHours(i);
                var stopTime = startTime.AddHours(1);

                var priceInLocalCurrency = prices[i].Value * exchangeRate / 1000;
                var priceInEuro = prices[i].Value / 1000;
                var priceInLocalCurrencyAfterSubsidy = priceInLocalCurrency;
                if (IsNorwegianPriceArea(priceArea.EIC_Code))
                {
                    var priceAfterSubsidy = (priceInLocalCurrency - (decimal)73.0) * (decimal)0.90;
                    if (priceAfterSubsidy > 0)
                    {
                        priceInLocalCurrencyAfterSubsidy = priceAfterSubsidy;
                    }
                }
                var priceInLocalCurrencyIncludingVAT = priceInLocalCurrency * (1 + priceArea.VATRate / 100);
                var priceInLocalCurrencyAfterSubsidyIncludingVAT = priceInLocalCurrencyAfterSubsidy * (1 + priceArea.VATRate / 100);

                var insertEnergyPriceCommand = new InsertEnergyPriceCommand(priceInLocalCurrencyIncludingVAT, priceInLocalCurrencyAfterSubsidyIncludingVAT, priceInEuro, startTime, stopTime, "NOK", exchangeRate, priceArea.VATRate, priceArea.EnergyPriceAreaId);
                await commandExecutor.ExecuteAsync(insertEnergyPriceCommand, cancellationToken);
            }
        }
    }

    private static bool IsNorwegianPriceArea(string eicCode)
        => norwegianPriceAreas.Contains(eicCode);
}
