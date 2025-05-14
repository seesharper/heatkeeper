SELECT
    n.Id,
    n.Name,    
    CASE WHEN ns.Id IS NOT NULL THEN 1 ELSE 0 END AS IsSubscribed
FROM Notifications n 
LEFT OUTER JOIN NotificationSubscriptions ns ON ns.NotificationId = n.Id AND ns.UserId = @UserId

