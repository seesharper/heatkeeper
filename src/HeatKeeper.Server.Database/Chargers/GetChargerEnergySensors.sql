SELECT
    s.Id,
    s.Name,
    s.ExternalId
FROM
    Sensors s
WHERE
    s.Id NOT IN (
        SELECT EnergySensorId
        FROM Chargers
        WHERE EnergySensorId IS NOT NULL
          AND Id != @ChargerId
    )
ORDER BY
    s.Name
