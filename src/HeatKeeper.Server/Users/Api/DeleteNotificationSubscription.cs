namespace HeatKeeper.Server.Users.Api;

[RequireUserRole]
[Delete("/api/notification-subscriptions/{id}")]
public record DeleteNotificationSubscription(long Id) : DeleteCommand;

public class DeleteNotificationSubscriptionHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteNotificationSubscription>
{
    public async Task HandleAsync(DeleteNotificationSubscription command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteNotificationSubscription, command);
}