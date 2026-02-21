SELECT
    s.Id AS SensorId,
    l.EnergyPriceAreaId,
    l.FixedEnergyPrice,
    l.UseFixedEnergyPrice
FROM Sensors s
INNER JOIN Zones z ON s.ZoneId = z.Id
INNER JOIN Locations l ON z.LocationId = l.Id
WHERE s.ExternalId = @ExternalSensorId
