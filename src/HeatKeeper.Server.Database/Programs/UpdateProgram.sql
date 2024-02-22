UPDATE Programs
SET
    ActiveScheduleId = @ActiveScheduleId,
    Name = @Name,
    Description = @Description
WHERE Id = @ProgramId    