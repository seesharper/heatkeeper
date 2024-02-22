SELECT 
    sp.Id, 
    sp.ScheduleId,
    sp.ZoneId, 
    sp.Value, 
    sp.Hysteresis
FROM 
    SetPoints sp 
WHERE 
    sp.Id = @Id