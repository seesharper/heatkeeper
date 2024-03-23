SELECT
    EXISTS(
        SELECT
            1
        FROM
            PushSubscriptions
        WHERE
            Endpoint = @Endpoint
    )