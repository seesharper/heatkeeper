SELECT
    z.Id AS ZoneId,
    z.Name,
    ltm.Value AS Temperature,
    ltm.Updated,
    lhm.Value AS Humidity
FROM Zones z
INNER JOIN LatestZoneMeasurements ltm on z.Id = ltm.ZoneId AND ltm.MeasurementType = 1
LEFT OUTER JOIN LatestZoneMeasurements lhm on z.id = lhm.ZoneId AND lhm.MeasurementType = 2
INNER JOIN Locations l ON z.LocationId = l.Id
INNER JOIN UserLocations UL on l.Id = UL.LocationId AND UL.UserId = @UserId
ORDER By z.Name