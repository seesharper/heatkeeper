namespace HeatKeeper.Server.PushSubscriptions;

[RequireBackgroundRole]
public record GetPushSubscriptionByUserQuery(long UserId) : IQuery<PushSubscriptionDetails[]>;

public class GetPushSubscriptionByUserQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetPushSubscriptionByUserQuery, PushSubscriptionDetails[]>
{
    public async Task<PushSubscriptionDetails[]> HandleAsync(GetPushSubscriptionByUserQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<PushSubscriptionDetails>(sqlProvider.GetPushSubscriptionByUser, query)).ToArray();
}