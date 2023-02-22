INSERT INTO SetPoints
(
    ScheduleId,
    ZoneId,
    Value,
    Hysteresis
)
VALUES
(
    @ScheduleId,
    @ZoneId,
    @Value,
    @Hysteresis
)