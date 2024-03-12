SELECT
    l.Id,
    l.Name,
    l.ActiveProgramId,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultOutsideZoneId) OutsideTemperature,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultInsideZoneId) InsideTemperature
 FROM Locations l