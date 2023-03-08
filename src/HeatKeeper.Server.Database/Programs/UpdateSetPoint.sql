UPDATE SetPoints
SET 
    Value = @Value,
    Hysteresis = @Hysteresis
WHERE 
    Id = @SetPointId    