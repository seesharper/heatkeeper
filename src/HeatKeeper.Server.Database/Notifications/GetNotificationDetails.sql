SELECT
    n.Id,    
    n.Name,
    n.Description,
    n.CronExpression,
    n.HoursToSnooze,
    n.LastSent,
    n.NotificationType
FROM Notifications n
WHERE n.Id = @NotificationId
