SELECT
    l.Id,
    l.Name,
    z.Name AS ZoneName,
    l.Description,
    l.MqttTopic,
    l.OnPayload,
    l.OffPayload,
    l.Enabled
FROM
    Lights l
    INNER JOIN Zones z ON l.ZoneId = z.Id
WHERE
    l.Id = @LightId;
