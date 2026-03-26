INSERT INTO LatestSensorMeasurements(SensorId, MeasurementType, Value, Updated)
SELECT s.Id, @MeasurementType, @Value, @Updated
FROM Sensors s WHERE s.ExternalId = @ExternalId
ON CONFLICT(SensorId, MeasurementType) DO UPDATE SET Value = excluded.Value, Updated = excluded.Updated
