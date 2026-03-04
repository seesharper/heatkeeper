SELECT
    datetime(strftime('%Y-%m-01', ec.Hour)) AS Timestamp,
    SUM(ec.PowerImport) AS PowerImport,
    SUM(ec.CostInLocalCurrency) AS CostInLocalCurrency,
    SUM(ec.CostInLocalCurrencyAfterSubsidy) AS CostInLocalCurrencyAfterSubsidy,
    SUM(ec.CostInLocalCurrencyWithFixedPrice) AS CostInLocalCurrencyWithFixedPrice
FROM EnergyCosts ec
INNER JOIN Sensors s ON ec.SensorId = s.Id
WHERE s.ZoneId = @ZoneId
  AND ec.Hour >= @FromDateTime
  AND ec.Hour < @ToDateTime
GROUP BY strftime('%Y-%m', ec.Hour)
ORDER BY Timestamp
