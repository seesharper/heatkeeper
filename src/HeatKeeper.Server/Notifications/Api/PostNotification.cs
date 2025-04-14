namespace HeatKeeper.Server.Notifications.Api;

[RequireUserRole]
[Post("/api/notifications")]
public record PostNotificationCommand(
    string Name,
    string Description,
    NotificationType NotificationType,
    string CronExpression,
    long HoursToSnooze) : PostCommand, INotificationCommand;

public class PostNotificationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostNotificationCommand>
{
    public async Task HandleAsync(PostNotificationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertNotification, command);
}