using HeatKeeper.Server.Notifications.Api;

namespace HeatKeeper.Server.Notifications;

public class WhenNotificationIsPatched(ICommandExecutor commandExecutor, ICommandHandler<PatchNotificationCommand> handler) : ICommandHandler<PatchNotificationCommand>
{
    public async Task HandleAsync(PatchNotificationCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        await commandExecutor.ExecuteAsync(new RemoveNotificationFromJanitorCommand(command.Id), cancellationToken);
        await commandExecutor.ExecuteAsync(new AddNotificationToJanitorCommand(command.Id, command.NotificationType, command.CronExpression), cancellationToken);
    }
}
