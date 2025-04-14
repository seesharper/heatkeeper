namespace HeatKeeper.Server.Notifications;

[RequireAdminRole]
public record DeleteAllNotificationConditionsCommand(long NotificationId);

public class DeleteAllNotificationConditions(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteAllNotificationConditionsCommand>
{
    public async Task HandleAsync(DeleteAllNotificationConditionsCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteAllNotificationConditions, command);
}