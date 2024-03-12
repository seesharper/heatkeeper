SELECT
    z.Id AS ZoneId,
    z.Name,
    ltm.Value AS Temperature,
    ltm.Updated,
    lhm.Value AS Humidity
FROM Zones z
INNER JOIN LatestZoneMeasurements ltm on z.Id = ltm.ZoneId AND ltm.MeasurementType = 1
LEFT OUTER JOIN LatestZoneMeasurements lhm on z.id = lhm.ZoneId AND lhm.MeasurementType = 2
WHERE z.LocationId = @LocationId
ORDER By z.Name