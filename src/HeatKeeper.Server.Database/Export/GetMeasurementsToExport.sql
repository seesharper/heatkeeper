SELECT
    m.Id,
    m.MeasurementType,
    m.RetentionPolicy,
    m.Value,
    m.Created,
    z.Name as Zone,
    l.Name as Location,
    s.ExternalId AS ExternalSensorId
FROM Measurements m
INNER JOIN Sensors s ON m.SensorId = s.Id
INNER JOIN Zones z on s.ZoneId = z.Id
INNER JOIN Locations l on l.Id = z.LocationId
WHERE m.Exported IS NULL