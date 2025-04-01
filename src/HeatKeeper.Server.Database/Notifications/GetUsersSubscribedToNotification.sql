SELECT
    n.UserId,
    n.Enabled,
    n.HoursToSnooze,
    n.LastSent
FROM Notifications n
WHERE n.NotificationType = @NotificationType