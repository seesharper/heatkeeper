SELECT
    s.Id,
    s.Name,
    s.Description,
    s.ExternalId,    
    s.LastSeen,
    s.EnableDeadSensorNotification,
    s.MinutesBeforeSensorIsConsideredDead,
    z.Name AS ZoneName
FROM
    Sensors s
    LEFT OUTER JOIN Zones z ON s.ZoneId = z.Id
WHERE
    s.Id = @SensorId;