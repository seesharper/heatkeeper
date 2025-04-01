SELECT
    n.Id,    
    n.Name,
    n.Description,
    n.CronExpression,
    n.HoursToSnooze,
    n.LastSent,
    n.NotificationType,
    n.Enabled
FROM Notifications n
WHERE n.Id = @NotificationId
