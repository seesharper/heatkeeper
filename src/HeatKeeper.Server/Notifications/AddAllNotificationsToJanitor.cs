namespace HeatKeeper.Server.Notifications;
[RequireBackgroundRole]
public record AddAllNotificationsToJanitorCommand();

public class AddAllNotificationsToJanitor(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<AddAllNotificationsToJanitorCommand>
{
    public async Task HandleAsync(AddAllNotificationsToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        var allScheduledNotifications = await queryExecutor.ExecuteAsync(new GetAllScheduledNotificationsQuery(), cancellationToken);

        foreach (var scheduledNotification in allScheduledNotifications)
        {
            await commandExecutor.ExecuteAsync(new AddNotificationToJanitorCommand(scheduledNotification.Id, scheduledNotification.NotificationType, scheduledNotification.CronExpression), cancellationToken);
        }
    }
}