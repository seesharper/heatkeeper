UPDATE LatestZoneMeasurements
SET
    Value = @Value,
    Updated = @Updated
WHERE
    ZoneId = @ZoneId AND
    MeasurementType = @MeasurementType