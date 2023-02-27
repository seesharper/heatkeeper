SELECT
    Id,
    Name,
    Description,
    MqttTopic
FROM
    Zones z
WHERE LocationId = @LocationId
