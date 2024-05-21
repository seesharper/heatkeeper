UPDATE
    Zones
SET
    NAME = @name,
    description = @description,
    locationId = @locationId
WHERE
    id = @ZoneId