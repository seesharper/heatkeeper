UPDATE
    Heaters
SET
    Enabled = 0,
    DisabledReason = @DisabledReason
WHERE
    Id = @HeaterId
