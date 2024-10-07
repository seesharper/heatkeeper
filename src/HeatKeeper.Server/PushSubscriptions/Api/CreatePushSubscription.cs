namespace HeatKeeper.Server.PushSubscriptions.Api;

public record Keys(string p256dh, string auth);

[RequireUserRole]
[Post("api/pushSubscriptions/")]
public record CreatePushSubscriptionCommand(string Endpoint, Keys Keys);

public class CreatePushSubscription(IUserContext userContext, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, TimeProvider timeProvider) : ICommandHandler<CreatePushSubscriptionCommand>
{
    public async Task HandleAsync(CreatePushSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var pushSubscriptionExistsQuery = new PushSubscriptionExistsQuery(command.Endpoint);
        var pushSubscriptionExists = await queryExecutor.ExecuteAsync(pushSubscriptionExistsQuery, cancellationToken);

        if (pushSubscriptionExists)
        {
            var updateLastSeenOnPushSubscriptionCommand = new UpdateLastSeenOnPushSubscriptionCommand(command.Endpoint, timeProvider.GetUtcNow().UtcDateTime);
            await commandExecutor.ExecuteAsync(updateLastSeenOnPushSubscriptionCommand, cancellationToken);
        }
        else
        {
            var insertPushSubscriptionCommand = new InsertPushSubscriptionCommand(command.Endpoint, command.Keys.p256dh, command.Keys.auth, userContext.Id, timeProvider.GetUtcNow().UtcDateTime);
            await commandExecutor.ExecuteAsync(insertPushSubscriptionCommand, cancellationToken);
        }
    }
}