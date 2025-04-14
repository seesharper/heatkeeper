namespace HeatKeeper.Server.Notifications.Api;

[RequireUserRole]
[Patch("/api/notifications/{id}")]
public record PatchNotificationCommand(
    long Id,
    string Name,
    string Description,
    NotificationType NotificationType,
    string CronExpression,
    long HoursToSnooze) : PatchCommand, INotificationCommand;

public class PatchNotificationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PatchNotificationCommand>
{
    public async Task HandleAsync(PatchNotificationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateNotification, command);
}
