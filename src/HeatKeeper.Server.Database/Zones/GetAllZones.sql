SELECT
    Id,
    Name,
    Description
FROM
    Zones z
WHERE LocationId = @LocationId
