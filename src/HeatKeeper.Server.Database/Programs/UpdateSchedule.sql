UPDATE Schedules
SET 
    Name = @Name,
    CronExpression = @CronExpression
WHERE 
    Id = @ScheduleId