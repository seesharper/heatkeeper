SELECT
    m.Value,
    m.Created
FROM
    Measurements m
    INNER JOIN Sensors S ON m.SensorId = S.Id
WHERE
    MeasurementType = @MeasurementType
    AND s.ZoneId = @ZoneId
    AND m.Created >= @since
ORDER BY
    created