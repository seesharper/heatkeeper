SELECT 
    sp.ZoneId,
    sp.Value,
    sp.Hysteresis
FROM 
    SetPoints sp
INNER JOIN 
    Schedules s
ON 
    sp.ScheduleId = s.Id
INNER JOIN 
    Programs p 
ON 
    s.ProgramId = p.Id AND
    p.ActiveScheduleID = sp.Id
INNER JOIN 
    Locations l
ON 
    p.LocationId = l.Id AND 
    l.ActiveProgramId = p.Id        

