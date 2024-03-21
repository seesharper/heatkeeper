using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.PushSubscriptions;

[RequireUserRole]
public record UpdateLastSeenOnPushSubscriptionCommand(string Endpoint, DateTime LastSeen);

public class UpdateLastSeenOnPushSubscriptionCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateLastSeenOnPushSubscriptionCommand>
{
    public async Task HandleAsync(UpdateLastSeenOnPushSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.UpdateLastSeenOnPushSubscription, command);
    }
}

