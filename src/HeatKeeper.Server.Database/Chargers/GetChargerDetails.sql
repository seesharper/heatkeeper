SELECT
    c.Id,
    c.Name,
    z.Name AS ZoneName,
    c.Description,
    c.MqttTopic,
    c.OnPayload,
    c.OffPayload,
    c.ChargerState
FROM
    Chargers c
    INNER JOIN Zones z ON c.ZoneId = z.Id
WHERE
    c.Id = @ChargerId;
