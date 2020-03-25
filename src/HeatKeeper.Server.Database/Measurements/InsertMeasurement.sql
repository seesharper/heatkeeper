INSERT INTO Measurements (SensorId, MeasurementType, Value, Created)
SELECT
    Id,
    @MeasurementType,
    @Value,
    @Created
FROM
    Sensors s
WHERE s.ExternalId = @SensorId
