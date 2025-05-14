namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record SendScheduledNotificationCommand(long NotificationId, NotificationType NotificationType);

public class SendScheduledNotificationCommandHandler(ICommandExecutor commandExecutor) : ICommandHandler<SendScheduledNotificationCommand>
{
    public async Task HandleAsync(SendScheduledNotificationCommand command, CancellationToken cancellationToken = default)
    {
        if (command.NotificationType == NotificationType.DeadSensors)
        {
            await commandExecutor.ExecuteAsync(new SendDeadSensorsNotificationCommand(command.NotificationId), cancellationToken);
        }
    }
}