INSERT INTO
    Chargers (
        ZoneId,
        Name,
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
