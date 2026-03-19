SELECT Hour AS Timestamp, Temperature
FROM ZoneTemperatures
WHERE ZoneId = @ZoneId
  AND Hour >= @FromDateTime
  AND Hour < @ToDateTime
ORDER BY Hour
