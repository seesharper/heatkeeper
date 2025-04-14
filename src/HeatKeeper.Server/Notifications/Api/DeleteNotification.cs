namespace HeatKeeper.Server.Notifications.Api;

[RequireUserRole]
[Delete("/api/notifications/{id}")]
public record DeleteNotificationCommand(long Id) : DeleteCommand;

public class DeleteNotificationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteNotificationCommand>
{
    public async Task HandleAsync(DeleteNotificationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteNotification, command);
}