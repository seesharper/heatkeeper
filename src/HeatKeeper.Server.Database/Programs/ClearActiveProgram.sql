UPDATE Locations
SET
    ActiveProgramId = null
WHERE
    ActiveProgramId = @ProgramId;