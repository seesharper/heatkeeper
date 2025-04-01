SELECT
    ps.Endpoint,
    ps.P256DH,
    ps.Auth,
    ps.UserId,
    ps.LastSeen
FROM
    PushSubscriptions ps
WHERE UserId = @UserId