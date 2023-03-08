UPDATE Programs
SET
    ActiveScheduleId = @ActiveScheduleId,
    Name = @Name
WHERE Id = @ProgramId    