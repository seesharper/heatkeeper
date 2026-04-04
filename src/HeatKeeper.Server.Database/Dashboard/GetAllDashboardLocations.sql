SELECT
    l.Id,
    l.Name,
    p.Name as ProgramName,
    s.Name as ScheduleName,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultOutsideZoneId) OutsideTemperature,
    (SELECT lzm.Value FROM LatestZoneMeasurements lzm WHERE lzm.MeasurementType = 1 and lzm.ZoneId = l.DefaultInsideZoneId) InsideTemperature
 FROM Locations l
 INNER JOIN Programs p on p.Id = l.ActiveProgramId
 INNER JOIN Schedules s on p.ActiveScheduleId = s.Id
 INNER JOIN UserLocations ul on ul.LocationId = l.Id AND ul.UserId = @UserId
