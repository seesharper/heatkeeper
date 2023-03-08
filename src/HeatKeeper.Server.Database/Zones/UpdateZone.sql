UPDATE Zones
SET
    name = @name,
    description = @description,
    mqttTopic = @mqttTopic
WHERE
    id = @ZoneId