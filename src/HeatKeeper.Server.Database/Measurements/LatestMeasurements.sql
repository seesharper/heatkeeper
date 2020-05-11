SELECT
    m.Id,
    s.ExternalId AS ExternalSensorId,
    m.MeasurementType,
    m.Value,
    m.Exported,
    m.Created
FROM Measurements m
INNER JOIN Sensors s ON m.SensorId = s.Id ORDER BY m.Created DESC LIMIT @limit