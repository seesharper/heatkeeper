SELECT
    Id,
    Name,
    CronExpression
FROM
    Schedules
WHERE 
    ProgramId = @ProgramId        