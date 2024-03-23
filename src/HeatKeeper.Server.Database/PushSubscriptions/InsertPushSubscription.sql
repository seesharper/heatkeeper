INSERT INTO
    PushSubscriptions (
        Endpoint,
        P256DH,
        Auth,
        UserId,
        LastSeen
    )
VALUES
    (
        @Endpoint,
        @P256DH,
        @Auth,
        @UserId,
        @LastSeen
    )