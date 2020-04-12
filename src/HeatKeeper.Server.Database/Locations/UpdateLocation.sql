UPDATE Locations
SET
    name = @name,
    description = @description
WHERE
    id = @LocationId