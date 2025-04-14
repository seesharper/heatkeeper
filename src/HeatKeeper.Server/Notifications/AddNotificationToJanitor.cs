using HeatKeeper.Server.Schedules;
using Janitor;

namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record AddNotificationToJanitorCommand(long NotificationId, NotificationType NotificationType, string CronExpression);

public class AddNotificationToJanitor(IJanitor janitor) : ICommandHandler<AddNotificationToJanitorCommand>
{
    private const string Prefix = "ScheduledNotification";

    public Task HandleAsync(AddNotificationToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        janitor.Schedule(builder =>
        {
            builder
                .WithName($"{Prefix}_{command.NotificationId}")
                .WithSchedule(new CronSchedule(command.CronExpression))
                .WithStateHandler(async (TaskState state) => { })
                .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken ct) =>
                {
                    await commandExecutor.ExecuteAsync(new SendScheduledNotificationCommand(command.NotificationId, command.NotificationType), ct);
                });
        });

        return Task.CompletedTask;
    }
}


