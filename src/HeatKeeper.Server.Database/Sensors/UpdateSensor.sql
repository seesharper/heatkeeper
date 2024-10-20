UPDATE
    Sensors
SET
    NAME = @Name,
    Description = @Description,
    EnableDeadSensorNotification = @EnableDeadSensorNotification,
    MinutesBeforeSensorIsConsideredDead = @MinutesBeforeSensorIsConsideredDead
WHERE
    Id = @SensorId