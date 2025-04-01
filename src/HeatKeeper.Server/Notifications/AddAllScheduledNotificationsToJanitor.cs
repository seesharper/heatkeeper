
using HeatKeeper.Server.Schedules;
using Janitor;

namespace HeatKeeper.Server.Notifications;

public record AddAllScheduledNotificationsToJanitorCommand();

public class AddAllScheduledNotificationsToJanitorCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, IJanitor janitor) : ICommandHandler<AddAllScheduledNotificationsToJanitorCommand>
{
    public async Task HandleAsync(AddAllScheduledNotificationsToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        var allScheduledNotifications = await queryExecutor.ExecuteAsync(new GetAllScheduledNotificationsQuery(), cancellationToken);
        foreach (var scheduledNotification in allScheduledNotifications)
        {
            janitor.Schedule(builder =>
            {
                builder
                    .WithName($"ScheduledNotification_{scheduledNotification.Id}")
                    .WithSchedule(new CronSchedule(scheduledNotification.CronExpression))
                    .WithScheduledTask(async (CancellationToken cancellationToken) =>
                    {
                        await commandExecutor.ExecuteAsync(new SendScheduledNotificationCommand(scheduledNotification), cancellationToken);
                    });

            });
        }
    }
}