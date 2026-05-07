SELECT
    Id,
    Name
FROM
    Chargers
WHERE
    ZoneId = @ZoneId
ORDER BY
    Name
