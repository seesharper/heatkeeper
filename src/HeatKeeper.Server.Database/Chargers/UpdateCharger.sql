UPDATE
    Chargers
SET
    ZoneId      = @ZoneId,
    Name        = @Name,
    Description = @Description,
    MqttTopic   = @MqttTopic,
    OnPayload   = @OnPayload,
    OffPayload  = @OffPayload
WHERE
    Id = @ChargerId
