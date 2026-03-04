SELECT ZoneId, Temperature, Hour, LastUpdate
FROM ZoneTemperatures
WHERE ZoneId = @ZoneId
ORDER BY Hour
