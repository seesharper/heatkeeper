namespace HeatKeeper.Server.PushSubscriptions;

[RequireBackgroundRole]
public record GetPushSubscriptionsByLocationQuery(long LocationId) : IQuery<PushSubscriptionDetails[]>;

public class GetAllSubscriptionsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetPushSubscriptionsByLocationQuery, PushSubscriptionDetails[]>
{
    public async Task<PushSubscriptionDetails[]> HandleAsync(GetPushSubscriptionsByLocationQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<PushSubscriptionDetails>(sqlProvider.GetPushSubscriptionsByLocation, query)).ToArray();
}

public record PushSubscriptionDetails(long Id, string Endpoint, string P256dh, string Auth, DateTime LastSeen);