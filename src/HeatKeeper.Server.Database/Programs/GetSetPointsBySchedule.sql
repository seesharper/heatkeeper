SELECT 
    Id,
    Value,
    Hysteresis,
    ZoneId
FROM 
    SetPoints
WHERE 
    ScheduleId = @ScheduleId        