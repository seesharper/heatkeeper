
namespace HeatKeeper.Server.Notifications;


public record SendScheduledNotificationCommand(ScheduledNotification ScheduledNotification);

public class SendScheduledNotificationCommandHandler(ICommandExecutor commandExecutor) : ICommandHandler<SendScheduledNotificationCommand>
{
    public async Task HandleAsync(SendScheduledNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var scheduledNotification = command.ScheduledNotification;
        if (scheduledNotification.NotificationType == NotificationType.DeadSensors)
        {
            await commandExecutor.ExecuteAsync(new SendDeadSensorsNotificationCommand(scheduledNotification.Id), cancellationToken);
        }
    }
}