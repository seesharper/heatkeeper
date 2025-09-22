UPDATE Locations
SET
    name = @name,
    description = @description,
    defaultOutsideZoneId = @defaultOutsideZoneId,
    defaultInsideZoneId = @defaultInsideZoneId,
    longitude = @longitude,
    latitude = @latitude
WHERE
    id = @Id