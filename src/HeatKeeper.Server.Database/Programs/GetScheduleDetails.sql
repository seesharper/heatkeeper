SELECT 
    Id, 
    ProgramId, 
    Name, 
    CronExpression 
FROM 
    Schedules 
WHERE 
    Id = @Id