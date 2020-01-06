SELECT
    Id,
    ExternalId,
    ZoneId,
    Name,
    Description
FROM
    Sensors
WHERE
    ZoneId = @ZoneId OR ZoneId IS NULL