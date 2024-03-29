SELECT
    z.Id,
    z.name,
    z.description,
    z.mqttTopic,
    (l.DefaultOutsideZoneId = z.Id) as IsDefaultOutsideZone,
    (l.DefaultInsideZoneId = z.Id) as IsDefaultInsideZone,
    z.LocationId

FROM
    Zones as z
INNER JOIN
    Locations as l
ON
    z.LocationId = l.Id and
    z.Id = @ZoneId