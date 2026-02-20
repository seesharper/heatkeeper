SELECT
    h.MqttTopic AS Topic,
    h.OnPayload,
    h.OffPayload,
    h.HeaterState
FROM
    Heaters h
WHERE
    h.ZoneId = @ZoneId