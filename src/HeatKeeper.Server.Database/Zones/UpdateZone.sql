UPDATE Zones
SET
    name = @name,
    description = @description
WHERE
    id = @ZoneId