SELECT 
    Id,
    Value,
    Hysteresis
FROM 
    SetPoints
WHERE 
    ScheduleId = @ScheduleId        