SELECT
    z.Id,
    z.name,
    z.description,
    z.LocationId
FROM
    Zones z
WHERE
    z.Id = @ZoneId