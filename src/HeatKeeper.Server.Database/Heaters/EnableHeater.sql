UPDATE
    Heaters
SET
    HeaterState = 0,
    DisabledReason = 0
WHERE
    Id = @HeaterId
