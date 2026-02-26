SELECT DISTINCT
    s.Id,
    s.Name
FROM EnergyCosts ec
INNER JOIN Sensors s ON ec.SensorId = s.Id
INNER JOIN Zones z ON s.ZoneId = z.Id
WHERE z.LocationId = @LocationId
