UPDATE Notifications
SET        
       CronExpression = @CronExpression,
       HoursToSnooze = @HoursToSnooze,
       Name = @Name,
       Description = @Description
 WHERE Id = @Id 