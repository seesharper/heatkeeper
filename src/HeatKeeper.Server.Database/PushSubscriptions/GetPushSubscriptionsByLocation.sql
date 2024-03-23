SELECT
    ps.Endpoint,
    ps.P256DH,
    ps.Auth,
    ps.UserId,
    ps.LastSeen
FROM
    PushSubscriptions ps
    INNER JOIN UserLocations UL ON ps.UserId = UL.UserId
    AND ul.LocationId = @LocationId