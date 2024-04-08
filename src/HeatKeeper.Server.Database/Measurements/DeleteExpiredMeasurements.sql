DELETE FROM
    Measurements
WHERE
    Created <= @RetentionDate
    AND RetentionPolicy = @RetentionPolicy