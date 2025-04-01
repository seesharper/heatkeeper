UPDATE
    Sensors
SET
    NAME = @Name,
    Description = @Description,
    ExternalId = @ExternalId,
    MinutesBeforeConsideredDead = @MinutesBeforeConsideredDead
WHERE
    Id = @SensorId