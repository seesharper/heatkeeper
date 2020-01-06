UPDATE Locations
SET
    DefaultOutsideZoneId = null
WHERE
    Id = @Locationid AND
    DefaultOutsideZoneId = @ZoneId