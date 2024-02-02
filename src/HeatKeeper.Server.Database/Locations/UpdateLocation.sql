UPDATE Locations
SET
    name = @name,
    description = @description,
    defaultOutsideZoneId = @defaultOutsideZoneId,
    defaultInsideZoneId = @defaultInsideZoneId
WHERE
    id = @Id