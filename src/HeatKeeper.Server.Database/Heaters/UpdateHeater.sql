UPDATE
    Heaters
SET
    NAME = @Name,
    Description = @Description,
    MqttTopic = @MqttTopic,
    OnPayload = @OnPayload,
    OffPayload = @OffPayload,
    HeaterState = @HeaterState
WHERE
    Id = @HeaterId