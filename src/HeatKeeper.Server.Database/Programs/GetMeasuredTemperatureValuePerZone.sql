SELECT 
    z.Id AS ZoneId,
    AVG(m.Value)
FROM 
    Measurements m
INNER JOIN 
    Sensors s
ON 
    m.SensorId = s.Id
INNER JOIN 
    Zones z
ON 
    s.ZoneId = z.Id
WHERE 
    m.Created >= @SinceUtcDateTime    
GROUP BY 
    z.Id