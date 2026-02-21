SELECT
    ep.PriceInLocalCurrency,
    ep.PriceInLocalCurrencyAfterSubsidy
FROM EnergyPrices ep
WHERE ep.EnergyPriceAreaId = @EnergyPriceAreaId
  AND ep.TimeStart <= @Hour
  AND ep.TimeEnd > @Hour
LIMIT 1
