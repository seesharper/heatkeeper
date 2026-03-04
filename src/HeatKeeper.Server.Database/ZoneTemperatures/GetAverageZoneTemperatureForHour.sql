SELECT COALESCE(AVG(m.Value), 0)
FROM Measurements m
INNER JOIN Sensors s ON m.SensorId = s.Id
WHERE s.ZoneId = @ZoneId
  AND m.MeasurementType = 1
  AND m.Created >= @HourStart
  AND m.Created < @HourEnd
