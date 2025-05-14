SELECT
    ns.UserId    
FROM NotificationSubscriptions ns
WHERE ns.NotificationId = @NotificationId