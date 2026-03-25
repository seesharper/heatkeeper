SELECT
    sp.Id,
    s.Name AS ScheduleName,
    sp.Value,
    sp.Hysteresis
FROM
    SetPoints sp
INNER JOIN
    Schedules s ON sp.ScheduleId = s.Id
INNER JOIN
    Programs p ON s.ProgramId = p.Id
INNER JOIN
    Locations l ON p.LocationId = l.Id AND l.ActiveProgramId = p.Id
WHERE
    sp.ZoneId = @ZoneId
