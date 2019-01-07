SELECT
    Id,
    Description
FROM
    Zones z
INNER JOIN
    Locations l
ON
    z.LocationId    =   l.Id AND
    l.Name          =   @location
