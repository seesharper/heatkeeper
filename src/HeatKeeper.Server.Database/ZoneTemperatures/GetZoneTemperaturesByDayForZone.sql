SELECT datetime(date(Hour)) AS Timestamp, AVG(Temperature) AS Temperature
FROM ZoneTemperatures
WHERE ZoneId = @ZoneId
  AND Hour >= @FromDateTime
  AND Hour < @ToDateTime
GROUP BY date(Hour)
ORDER BY Timestamp
