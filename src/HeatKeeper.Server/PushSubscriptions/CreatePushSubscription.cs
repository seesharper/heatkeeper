using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.PushSubscriptions;

public record Keys(string p256dh, string auth);

[RequireUserRole]
public record CreatePushSubscriptionCommand(string Endpoint, Keys Keys);

public class CreatePushSubscriptionCommandHandler(IUserContext userContext, ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : ICommandHandler<CreatePushSubscriptionCommand>
{
    public async Task HandleAsync(CreatePushSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        var pushSubscriptionExistsQuery = new PushSubscriptionExistsQuery(command.Endpoint);
        var pushSubscriptionExists = await queryExecutor.ExecuteAsync(pushSubscriptionExistsQuery, cancellationToken);

        if (pushSubscriptionExists)
        {
            var updateLastSeenOnPushSubscriptionCommand = new UpdateLastSeenOnPushSubscriptionCommand(command.Endpoint, DateTime.UtcNow);
            await commandExecutor.ExecuteAsync(updateLastSeenOnPushSubscriptionCommand, cancellationToken);
        }
        else
        {
            var insertPushSubscriptionCommand = new InsertPushSubscriptionCommand(command.Endpoint, command.Keys.p256dh, command.Keys.auth, userContext.Id, DateTime.UtcNow);
            await commandExecutor.ExecuteAsync(insertPushSubscriptionCommand, cancellationToken);
        }
    }
}