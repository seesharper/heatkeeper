UPDATE Locations
SET 
    ActiveProgramId = @ProgramId
WHERE 
    Id = (SELECT LocationId FROM Programs WHERE Id = @ProgramId)
