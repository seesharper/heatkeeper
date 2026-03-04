INSERT INTO ZoneTemperatures (ZoneId, Temperature, Hour, LastUpdate)
VALUES (@ZoneId, @Temperature, @Hour, @LastUpdate)
ON CONFLICT(ZoneId, Hour) DO UPDATE SET
    Temperature = @Temperature,
    LastUpdate = @LastUpdate;
