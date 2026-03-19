SELECT datetime(strftime('%Y-%m-01', Hour)) AS Timestamp, AVG(Temperature) AS Temperature
FROM ZoneTemperatures
WHERE ZoneId = @ZoneId
  AND Hour >= @FromDateTime
  AND Hour < @ToDateTime
GROUP BY strftime('%Y-%m', Hour)
ORDER BY Timestamp
