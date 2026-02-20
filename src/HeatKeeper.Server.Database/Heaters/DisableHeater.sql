UPDATE
    Heaters
SET
    HeaterState = 3,
    DisabledReason = @DisabledReason
WHERE
    Id = @HeaterId
