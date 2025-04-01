insert into Notifications (UserId, NotificationType, CronExpression, HoursToSnooze, Name,
                           Description)
values (@UserId, @NotificationType, @CronExpression, @HoursToSnooze, @Name, @Description);