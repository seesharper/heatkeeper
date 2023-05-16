SELECT     
    z.Name AS ZoneName,    
    l.Name AS LocationName
FROM 
Zones z
    INNER JOIN Locations l ON z.LocationId = l.Id
WHERE 
    z.Id = @ZoneId    