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
public record InsertPushSubscriptionCommand(
    string Endpoint,
    string P256DH,
    string Auth,
    long UserId,
    DateTime LastSeen);

public class InsertPushSubscriptionCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<InsertPushSubscriptionCommand>
{
    public async Task HandleAsync(InsertPushSubscriptionCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertPushSubscription, command);
}