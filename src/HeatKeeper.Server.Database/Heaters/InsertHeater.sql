INSERT INTO
    Heaters (
        ZoneId,
        NAME,
        Description,
        MqttTopic,
        OnPayload,
        OffPayload
    )
VALUES
    (
        @ZoneId,
        @Name,
        @Description,
        @MqttTopic,
        @OnPayload,
        @OffPayload
    );