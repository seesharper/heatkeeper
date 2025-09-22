SELECT
    l.Id,
    l.Name,
    l.Description,
    l.DefaultInsideZoneId,
    l.DefaultOutsideZoneId,
    l.ActiveProgramId,
    l.Longitude,
    l.Latitude
FROM Locations l
WHERE l.id = @id