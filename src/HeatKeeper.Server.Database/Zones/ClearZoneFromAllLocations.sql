UPDATE Locations
SET
    DefaultInsideZoneId = null
WHERE
    DefaultInsideZoneId = @ZoneId;

UPDATE Locations
SET
    DefaultOutsideZoneId = null
WHERE
    DefaultOutsideZoneId = @ZoneId;