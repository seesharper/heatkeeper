SELECT 
    n.Enabled,
    n.HoursToSnooze,
    n.LastSent
FROM Notifications n
WHERE n.Id = @NotificationId