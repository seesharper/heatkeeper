namespace HeatKeeper.Server.Users.Api;

[Post("/api/users/{userId}/notification-subscriptions")]
[RequireUserRole]
public record PostNotificationSubscriptionCommand(long UserId, long NotificationId) : PostCommand;

public class PostNotificationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostNotificationSubscriptionCommand>
{
    public async Task HandleAsync(PostNotificationSubscriptionCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertNotificationSubscription, command);
}
