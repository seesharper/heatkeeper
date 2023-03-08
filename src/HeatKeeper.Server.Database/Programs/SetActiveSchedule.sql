UPDATE Programs 
SET 
    ActiveScheduleId = @ScheduleId
WHERE 
    Id = (SELECT ProgramId FROM Schedules WHERE Id = @ScheduleId)
