namespace HeatKeeper.Server.Notifications;

[RequireAdminRole]
public record DeleteAllNotificationSubscriptionsCommand(long NotificationId);

public class DeleteAllNotificationSubscriptions(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteAllNotificationSubscriptionsCommand>
{
    public async Task HandleAsync(DeleteAllNotificationSubscriptionsCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteAllNotificationSubscriptions, command);
}