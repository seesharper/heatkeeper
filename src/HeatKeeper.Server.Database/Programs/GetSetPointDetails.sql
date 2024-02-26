SELECT
    sp.Id,
    z.Name as ZoneName,
    s.Name as ScheduleName,
    sp.Value,
    sp.Hysteresis
FROM
    SetPoints sp
INNER JOIN 
    Schedules s ON 
sp.ScheduleId = s.Id
INNER JOIN Zones z ON 
    sp.ZoneId = z.Id
WHERE
    sp.Id = @Id