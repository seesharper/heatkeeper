SELECT
    Id,
    UserId,
    CronExpression,
    NotificationType
FROM Notifications
WHERE CronExpression IS NOT NULL