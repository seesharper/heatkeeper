UPDATE Sensors
SET
    ZoneId = null
WHERE
    Id = @SensorId AND
    ZoneID = @ZoneID