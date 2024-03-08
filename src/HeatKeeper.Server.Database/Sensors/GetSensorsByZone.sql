SELECT
    Id,
    ExternalId,
    ZoneId,
    NAME,
    Description
FROM
    Sensors
WHERE
    ZoneId = @ZoneId