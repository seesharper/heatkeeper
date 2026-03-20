INSERT INTO ZoneTemperatures (ZoneId, Temperature, Hour, LastUpdate)
SELECT lzm.ZoneId, lzm.Value, @CurrentHour, @CurrentHour
FROM LatestZoneMeasurements lzm
WHERE lzm.MeasurementType = @MeasurementType
  AND NOT EXISTS (
    SELECT 1 FROM ZoneTemperatures zt
    WHERE zt.ZoneId = lzm.ZoneId AND zt.Hour = @CurrentHour
  )
