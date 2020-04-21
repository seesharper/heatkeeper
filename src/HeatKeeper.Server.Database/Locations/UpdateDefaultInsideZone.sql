UPDATE Locations
SET DefaultInsideZoneId = @ZoneId
WHERE Id = (SELECT LocationId from zones where id = @ZoneId)

