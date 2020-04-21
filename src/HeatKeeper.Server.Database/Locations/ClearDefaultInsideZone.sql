UPDATE Locations
SET
    DefaultInsideZoneId = null
WHERE
    Id = (SELECT LocationId from zones where id = @ZoneId) AND
    DefaultInsideZoneId = @ZoneId