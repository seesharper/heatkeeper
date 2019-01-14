INSERT INTO Measurements (SensorId, MeasurementType, Value)
SELECT
    Id,
    @MeasurementType,
    @Value
FROM
    Sensors s
WHERE s.ExternalId = @SensorId
