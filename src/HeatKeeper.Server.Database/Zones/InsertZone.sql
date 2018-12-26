INSERT INTO Zones(LocationId, Name, Description)
SELECT
    Id,
    @name,
    @description
FROM
    Locations
WHERE
    Name = @location
