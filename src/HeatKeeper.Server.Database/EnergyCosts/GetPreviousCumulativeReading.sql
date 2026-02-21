SELECT m.Value, m.Created
FROM Measurements m
INNER JOIN Sensors s ON m.SensorId = s.Id
WHERE s.ExternalId = @ExternalSensorId
  AND m.MeasurementType = 12
  AND m.Created < @Created
ORDER BY m.Created DESC
LIMIT 1
