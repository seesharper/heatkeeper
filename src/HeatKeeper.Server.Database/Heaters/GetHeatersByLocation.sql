SELECT
    h.Id,
    h.Name,
    h.HeaterState
FROM
    Heaters h
    INNER JOIN Zones z ON h.ZoneId = z.Id
    INNER JOIN Locations l ON z.LocationId = l.Id
WHERE
    l.Id = @LocationId
ORDER BY
    h.Name
