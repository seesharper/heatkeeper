SELECT
    z.Id,
    z.name,
    z.description,
    (l.DefaultOutsideZoneId = z.Id) as IsDefaultOutsideZone,
    (l.DefaultInsideZoneId = z.Id) as IsDefaultInsideZone

FROM
    Zones as z
INNER JOIN
    Locations as l
ON
    z.LocationId = l.Id and
    z.Id = @ZoneId