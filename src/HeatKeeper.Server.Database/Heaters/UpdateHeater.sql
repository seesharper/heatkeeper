UPDATE
    Heaters
SET
    NAME = @Name,
    Description = @Description,
    MqttTopic = @MqttTopic,
    OnPayload = @OnPayload,
    OffPayload = @OffPayload,
    Enabled = @Enabled
WHERE
    Id = @HeaterId