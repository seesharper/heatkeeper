SELECT
    l.Id,
    l.Name,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultOutsideZoneId) OutsideTemperature,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 2 and lzm.ZoneId = l.DefaultOutsideZoneId) OutsideHumidity,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultInsideZoneId) InsideTemperature,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 2 and lzm.ZoneId = l.DefaultInsideZoneId) InsideHumidity
 FROM Locations l