SELECT
    h.MqttTopic AS Topic,
    h.OnPayload,
    h.OffPayload
FROM
    Heaters h
WHERE
    h.ZoneId = @ZoneId