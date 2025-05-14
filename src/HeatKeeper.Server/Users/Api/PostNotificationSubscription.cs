namespace HeatKeeper.Server.Users.Api;

[Post("/api/notification-subscriptions")]
[RequireUserRole]
public record PostNotificationSubscriptionCommand(long NotificationId) : PostCommand;

public class PostNotificationCommandHandler(IUserContext userContext, IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostNotificationSubscriptionCommand>
{
    public async Task HandleAsync(PostNotificationSubscriptionCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertNotificationSubscription, new { UserId = userContext.Id, command.NotificationId });
}
