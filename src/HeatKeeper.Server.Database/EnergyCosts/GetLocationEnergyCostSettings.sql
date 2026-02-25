SELECT
    l.SmartMeterSensorId,
    l.EnergyCalculationStrategy
FROM Locations l
WHERE l.Id = @LocationId
