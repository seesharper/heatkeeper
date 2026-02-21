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
    energyPriceAreaId = @energyPriceAreaId
WHERE
    id = @Id