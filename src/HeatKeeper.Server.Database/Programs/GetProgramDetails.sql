SELECT 
    Id, 
    Name,
    Description,
    LocationId, 
    ActiveScheduleId 
FROM 
    Programs 
WHERE 
    Id = @Id