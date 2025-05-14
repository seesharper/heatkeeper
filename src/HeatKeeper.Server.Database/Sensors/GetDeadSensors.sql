SELECT 
    s.Id,
    s.Name,
    s.ExternalId,
    z.Name AS Zone,
    l.Name as Location,
    s.LastSeen
FROM 
    Sensors s
INNER JOIN 
    Zones z  
ON 
    s.ZoneId = z.Id AND
    DateTime(s.LastSeen, '+' || s.MinutesBeforeConsideredDead || ' minutes') < @Now
INNER JOIN 
    Locations l 
ON 
    z.LocationId = l.Id          

