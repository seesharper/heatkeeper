SELECT
    l.SmartMeterSensorId,
    l.EnergyCalculationStrategy,
    l.TimeZone
FROM Locations l
WHERE l.Id = @LocationId
