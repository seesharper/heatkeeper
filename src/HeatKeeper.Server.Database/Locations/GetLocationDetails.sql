SELECT
    l.Id,
    l.Name,
    l.Description,
    l.DefaultInsideZoneId,
    l.DefaultOutsideZoneId,
    l.ActiveProgramId,
    l.Longitude,
    l.Latitude,
    l.FixedEnergyPrice,
    l.UseFixedEnergyPrice
FROM Locations l
WHERE l.id = @id