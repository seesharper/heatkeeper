SELECT
    h.Id AS HeaterId,
    h.MqttTopic AS Topic,
    h.OnPayload,
    h.OffPayload,
    h.HeaterState
FROM
    Heaters h
WHERE
    h.ZoneId = @ZoneId