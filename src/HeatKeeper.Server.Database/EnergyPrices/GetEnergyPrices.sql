SELECT
    ep.PriceInLocalCurrency AS Price,
    ep.PriceInLocalCurrencyAfterSubsidy AS PriceAfterSubsidy,
    ep.TimeStart,
    ep.TimeEnd
FROM EnergyPrices ep
WHERE EnergyPriceAreaId = @EnergyPriceAreaId AND TimeStart >= @Date AND TimeStart < date(@Date, '+1 day')