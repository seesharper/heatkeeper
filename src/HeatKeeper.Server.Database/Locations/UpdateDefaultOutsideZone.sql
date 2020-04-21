UPDATE Locations
SET DefaultOutsideZoneId = @ZoneId
WHERE Id = (SELECT LocationId from zones where id = @ZoneId)
