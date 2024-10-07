SELECT
    EXISTS(
        SELECT
            1
        FROM
            locations
        WHERE
            NAME = @name
    )