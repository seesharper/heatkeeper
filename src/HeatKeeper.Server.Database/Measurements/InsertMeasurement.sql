INSERT INTO Measurements (SensorId, MeasurementType, RetentionPolicy, Value, Created)
SELECT
    Id,
    @MeasurementType,
    @RetentionPolicy,
    @Value,
    @Created
FROM
    Sensors s
WHERE s.ExternalId = @SensorId
