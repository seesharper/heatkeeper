SELECT
    z.id,
    z.Name
FROM
    Zones z
WHERE LocationId = (
    SELECT 
        l.id 
    FROM 
        Locations l 
    INNER JOIN 
        Programs p 
    ON 
        l.Id = p.LocationId 
    INNER JOIN 
        Schedules s 
    ON 
        p.Id = s.ProgramId AND 
        s.Id = @ScheduleId
    )
AND NOT EXISTS (
    SELECT 
        1 
    FROM 
        SetPoints sp 
    WHERE 
        sp.ScheduleId = @ScheduleId AND 
        sp.ZoneId = z.Id
    )
ORDER BY z.Name