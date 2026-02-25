UPDATE Locations
SET
    name = @name,
    description = @description,
    defaultOutsideZoneId = @defaultOutsideZoneId,
    defaultInsideZoneId = @defaultInsideZoneId,
    longitude = @longitude,
    latitude = @latitude,
    fixedEnergyPrice = @fixedEnergyPrice,
    useFixedEnergyPrice = @useFixedEnergyPrice,
    energyPriceAreaId = @energyPriceAreaId,
    smartMeterSensorId = @smartMeterSensorId,
    energyCalculationStrategy = @energyCalculationStrategy
WHERE
    id = @Id