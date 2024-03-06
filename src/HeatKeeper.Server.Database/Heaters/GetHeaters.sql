SELECT
    Id,
    NAME
FROM
    Heaters
WHERE
    ZoneId = @ZoneId
ORDER BY
    NAME