using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.PushSubscriptions;

[RequireBackgroundRole]
public record DeleteOldPushSubscriptionsCommand(DateTime Date);

public class DeleteOldPushSubscriptionsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteOldPushSubscriptionsCommand>
{
    public async Task HandleAsync(DeleteOldPushSubscriptionsCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteOldPushSubscriptions, command);
}
