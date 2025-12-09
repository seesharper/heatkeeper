SELECT
    Id,
    NAME
FROM
    Lights
WHERE
    ZoneId = @ZoneId
ORDER BY
    NAME
