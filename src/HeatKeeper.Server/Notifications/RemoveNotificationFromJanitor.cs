using Janitor;

namespace HeatKeeper.Server.Notifications;

[RequireAdminRole]
public record RemoveNotificationFromJanitorCommand(long NotificationId);

public class RemoveNotificationFromJanitor(IJanitor janitor) : ICommandHandler<RemoveNotificationFromJanitorCommand>
{
    public async Task HandleAsync(RemoveNotificationFromJanitorCommand command, CancellationToken cancellationToken = default)
        => await janitor.Delete($"ScheduledNotification_{command.NotificationId}");
}