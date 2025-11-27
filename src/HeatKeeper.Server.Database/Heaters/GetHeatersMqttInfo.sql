SELECT
    h.MqttTopic AS Topic,
    h.OnPayload,
    h.OffPayload,
    h.Enabled
FROM
    Heaters h
WHERE
    h.ZoneId = @ZoneId