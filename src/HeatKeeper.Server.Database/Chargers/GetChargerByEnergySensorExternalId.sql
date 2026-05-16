SELECT
    c.Id AS ChargerId,
    c.EnergyThreshold,
    c.ChargerState
FROM
    Chargers c
    INNER JOIN Sensors s ON c.EnergySensorId = s.Id
WHERE
    s.ExternalId = @SensorExternalId
    AND c.ChargerState IN (0, 1)
