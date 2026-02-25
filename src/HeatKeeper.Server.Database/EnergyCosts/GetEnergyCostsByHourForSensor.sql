SELECT
    ec.Hour AS Timestamp,
    SUM(ec.PowerImport) AS PowerImport,
    SUM(ec.CostInLocalCurrency) AS CostInLocalCurrency,
    SUM(ec.CostInLocalCurrencyAfterSubsidy) AS CostInLocalCurrencyAfterSubsidy,
    SUM(ec.CostInLocalCurrencyWithFixedPrice) AS CostInLocalCurrencyWithFixedPrice
FROM EnergyCosts ec
WHERE ec.SensorId = @SensorId
  AND ec.Hour >= @FromDateTime
  AND ec.Hour < @ToDateTime
GROUP BY ec.Hour
ORDER BY ec.Hour
