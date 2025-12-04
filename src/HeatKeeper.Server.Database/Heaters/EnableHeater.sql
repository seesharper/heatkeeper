UPDATE
    Heaters
SET
    Enabled = 1,
    DisabledReason = 0
WHERE
    Id = @HeaterId
