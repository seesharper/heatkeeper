SELECT 
    lzm.Value,
    lzm.Updated,
    lzm.ZoneId
FROM    
    Zones z
INNER JOIN 
    LatestZoneMeasurements lzm
WHERE             
    z.Id = lzm.ZoneId
GROUP BY
    lzm.ZoneId,
    lzm.Updated