SELECT
    s.Id,
    s.Name,
    s.Description,
    s.ExternalId,
    s.MinutesBeforeConsideredDead,
    s.LastSeen,
    z.Name AS ZoneName
FROM
    Sensors s
    LEFT OUTER JOIN Zones z ON s.ZoneId = z.Id
WHERE
    s.Id = @SensorId;