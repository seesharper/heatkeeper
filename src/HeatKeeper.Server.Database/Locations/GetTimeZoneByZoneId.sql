SELECT COALESCE(l.TimeZone, '') AS TimeZone
FROM Locations l
INNER JOIN Zones z ON z.LocationId = l.Id
WHERE z.Id = @ZoneId
