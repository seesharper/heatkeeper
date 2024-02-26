SELECT
    sp.Id,
    z.Name AS ZoneName,
    sp.Value
FROM
    SetPoints sp
INNER JOIN
    Zones z
ON sp.ZoneId = z.Id
WHERE
    ScheduleId = @ScheduleId