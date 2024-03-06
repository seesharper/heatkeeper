SELECT
    h.MqttTopic,
    h.OnPayload,
    h.OffPayload
FROM
    Heaters h
WHERE
    h.ZoneId = @ZoneId