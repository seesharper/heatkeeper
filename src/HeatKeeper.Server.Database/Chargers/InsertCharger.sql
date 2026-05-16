INSERT INTO
    Chargers (
        ZoneId,
        Name,
        Description,
        MqttTopic,
        OnPayload,
        OffPayload,
        EnergySensorId,
        EnergyThreshold
    )
VALUES
    (
        @ZoneId,
        @Name,
        @Description,
        @MqttTopic,
        @OnPayload,
        @OffPayload,
        @EnergySensorId,
        @EnergyThreshold
    );
