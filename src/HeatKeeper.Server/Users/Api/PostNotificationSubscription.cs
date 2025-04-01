using HeatKeeper.Server.Notifications;

namespace HeatKeeper.Server.Users.Api;


[Post("/api/users/{userId}/notifications")]
[RequireUserRole]
public record PostNotificationCommand(long UserId, NotificationType NotificationType, string CronExpression, long HoursToSnooze, string Name, string Description) : PostCommand;

public class PostNotificationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostNotificationCommand>
{
    public async Task HandleAsync(PostNotificationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertNotification, command);
}
