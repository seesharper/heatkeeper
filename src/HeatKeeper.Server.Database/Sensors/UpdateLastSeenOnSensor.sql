UPDATE Sensors
SET 
    LastSeen = @LastSeen
WHERE 
    ExternalId = @ExternalId  