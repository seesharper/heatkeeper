UPDATE Locations
SET
    DefaultInsideZoneId = null
WHERE
    Id = @Locationid AND
    DefaultInsideZoneId = @ZoneId