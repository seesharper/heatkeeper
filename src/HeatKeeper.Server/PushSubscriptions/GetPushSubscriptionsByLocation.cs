using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.PushSubscriptions;

[RequireBackgroundRole]
public record GetPushSubscriptionsByLocationQuery(long LocationId) : IQuery<SubscriptionsQueryResult[]>;

public class GetAllSubscriptionsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetPushSubscriptionsByLocationQuery, SubscriptionsQueryResult[]>
{
    public async Task<SubscriptionsQueryResult[]> HandleAsync(GetPushSubscriptionsByLocationQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<SubscriptionsQueryResult>(sqlProvider.GetPushSubscriptionsByLocation, query)).ToArray();
}

public record SubscriptionsQueryResult(long Id, string Endpoint, string P256dh, string Auth, DateTime LastSeen);