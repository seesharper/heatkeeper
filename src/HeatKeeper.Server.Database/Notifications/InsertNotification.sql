INSERT INTO Notifications 
(                              
    NotificationType,                              
    CronExpression,
    HoursToSnooze,
    Name,
    Description
)
VALUES 
(
    @NotificationType,
    @CronExpression,
    @HoursToSnooze,
    @Name,
    @Description
)
                         