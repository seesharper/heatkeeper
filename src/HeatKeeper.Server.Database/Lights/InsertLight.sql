INSERT INTO
    Lights (
        ZoneId,
        NAME,
        Description,
        MqttTopic,
        OnPayload,
        OffPayload,
        Enabled
    )
VALUES
    (
        @ZoneId,
        @Name,
        @Description,
        @MqttTopic,
        @OnPayload,
        @OffPayload,
        @Enabled
    );
