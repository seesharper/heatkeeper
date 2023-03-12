SELECT 
    z.Id AS ZoneId,
    AVG(m.Value) AS Value
FROM 
    Measurements m
INNER JOIN 
    Sensors s
ON 
    m.SensorId = s.Id AND 
    m.MeasurementType = 1
INNER JOIN 
    Zones z
ON 
    s.ZoneId = z.Id
WHERE 
    m.Created >= @SinceUtcDateTime    
GROUP BY 
    z.Id