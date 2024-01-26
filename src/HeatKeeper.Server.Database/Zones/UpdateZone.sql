UPDATE Zones
SET
    name = @name,
    description = @description,
    mqttTopic = @mqttTopic,
    locationId = @locationId
WHERE
    id = @ZoneId