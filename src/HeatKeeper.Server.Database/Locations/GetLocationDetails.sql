SELECT
    l.Id,
    l.Name,
    l.Description,
    l.DefaultInsideZoneId,
    l.DefaultOutsideZoneId
FROM Locations l
WHERE l.id = @id