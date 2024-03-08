SELECT
    Id,
    NAME,
    ExternalId,
    LastSeen
FROM
    Sensors
WHERE
    ZoneId IS NULL
ORDER BY
    LastSeen DESC