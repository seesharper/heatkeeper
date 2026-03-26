SELECT
    s.Id   AS SensorId,
    s.Name AS SensorName,
    lsm.Value   AS ActivePowerImport,
    lsm.Updated
FROM
    LatestSensorMeasurements lsm
INNER JOIN Sensors s ON lsm.SensorId  = s.Id
INNER JOIN Zones   z ON s.ZoneId      = z.Id
WHERE
    z.LocationId        = @LocationId
    AND lsm.MeasurementType = 5
