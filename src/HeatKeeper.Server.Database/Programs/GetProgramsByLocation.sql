SELECT 
    Id, 
    Name,
    ActiveScheduleId
FROM 
    Programs
WHERE 
    LocationId = @LocationId        