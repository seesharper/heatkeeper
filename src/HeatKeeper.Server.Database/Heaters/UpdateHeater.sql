UPDATE
    Heaters
SET
    NAME = @Name,
    Description = @Description,
    MqttTopic = @MqttTopic,
    OnPayload = @OnPayload,
    OffPayload = @OffPayload
WHERE
    Id = @HeaterId