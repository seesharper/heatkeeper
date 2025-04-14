using HeatKeeper.Server.Notifications.Api;

namespace HeatKeeper.Server.Notifications;

public class WhenNotificationIsDeleted(ICommandExecutor commandExecutor, ICommandHandler<DeleteNotificationCommand> handler) : ICommandHandler<DeleteNotificationCommand>
{
    public async Task HandleAsync(DeleteNotificationCommand command, CancellationToken cancellationToken = default)
    {
        await commandExecutor.ExecuteAsync(new DeleteAllNotificationSubscriptionsCommand(command.Id), cancellationToken);
        await commandExecutor.ExecuteAsync(new DeleteAllNotificationConditionsCommand(command.Id), cancellationToken);
        await commandExecutor.ExecuteAsync(new RemoveNotificationFromJanitorCommand(command.Id), cancellationToken);
        await handler.HandleAsync(command, cancellationToken);
    }
}