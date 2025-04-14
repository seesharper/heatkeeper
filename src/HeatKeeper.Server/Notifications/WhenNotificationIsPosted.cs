using HeatKeeper.Server.Notifications.Api;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Notifications;

public class WhenNotificationIsPosted(ICommandExecutor commandExecutor, ICommandHandler<PostNotificationCommand> handler) : ICommandHandler<PostNotificationCommand>
{
    public async Task HandleAsync(PostNotificationCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        var result = command.GetResult().Result;
        if (result is Created<ResourceId> created)
        {
            await commandExecutor.ExecuteAsync(new AddNotificationToJanitorCommand(created.Value.Id, command.NotificationType, command.CronExpression), cancellationToken);
        }
    }
}