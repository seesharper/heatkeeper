namespace HeatKeeper.Server.Notifications;

public interface INotificationCommand : IProblemCommand
{
    string Name { get; }
    string Description { get; }
    NotificationType NotificationType { get; }
    string CronExpression { get; }
    long HoursToSnooze { get; }
}

public class ValidateNotification(ICronExpressionValidator cronExpressionValidator) : IValidator<INotificationCommand>
{
    public Task Validate(INotificationCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            command.SetProblemResult("Name cannot be null or empty.", StatusCodes.Status400BadRequest);
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(command.CronExpression) || !cronExpressionValidator.Validate(command.CronExpression))
        {
            command.SetProblemResult($"The cron expression {command.CronExpression} is not valid for notification {command.Name}", StatusCodes.Status400BadRequest);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
