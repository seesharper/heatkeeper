UPDATE Sensors
SET
    Name = @Name,
    Description = @Description,
    ZoneId = @ZoneId
WHERE
    Id = @SensorId