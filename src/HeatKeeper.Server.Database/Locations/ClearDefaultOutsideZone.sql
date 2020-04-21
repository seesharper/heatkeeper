UPDATE Locations
SET
    DefaultOutsideZoneId = null
WHERE
    Id = (SELECT LocationId from zones where id = @ZoneId) AND
    DefaultOutsideZoneId = @ZoneId