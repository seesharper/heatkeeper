SELECT 
    lzm.Value,
    lzm.Updated,
    lzm.ZoneId
FROM    
    Zones z
INNER JOIN 
    LatestZoneMeasurements lzm
ON             
    z.Id = lzm.ZoneId AND 
    lzm.MeasurementType = 1  
GROUP BY
    lzm.ZoneId,
    lzm.Updated