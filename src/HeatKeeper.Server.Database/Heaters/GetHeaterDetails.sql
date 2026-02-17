SELECT
    h.Id,
    h.Name,
    z.Name AS ZoneName,
    h.Description,
    h.MqttTopic,
    h.OnPayload,
    h.OffPayload,
    h.HeaterState
FROM
    Heaters h
    INNER JOIN Zones z ON h.ZoneId = z.Id
WHERE
    h.Id = @HeaterId;