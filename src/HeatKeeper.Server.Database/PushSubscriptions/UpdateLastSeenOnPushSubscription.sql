UPDATE
    PushSubscriptions
SET
    LastSeen = @LastSeen
WHERE
    Endpoint = @Endpoint