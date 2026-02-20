INSERT INTO
    Heaters (
        ZoneId,
        NAME,
        Description,
        MqttTopic,
        OnPayload,
        OffPayload,
        HeaterState
    )
VALUES
    (
        @ZoneId,
        @Name,
        @Description,
        @MqttTopic,
        @OnPayload,
        @OffPayload,
        @HeaterState
    );