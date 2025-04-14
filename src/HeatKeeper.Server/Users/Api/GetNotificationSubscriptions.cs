namespace HeatKeeper.Server.Users.Api;

[Get("/api/users/{userId}/notifications")]
[RequireUserRole]
public record GetNotificationSubscriptionsQuery(long UserId) : IQuery<NotificationSubscriptionInfo[]>;

public record NotificationSubscriptionInfo(long Id, string Name, bool IsSubscribed);

public class GetNotificationSubscriptions(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationSubscriptionsQuery, NotificationSubscriptionInfo[]>
{
    public async Task<NotificationSubscriptionInfo[]> HandleAsync(GetNotificationSubscriptionsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationSubscriptionInfo>(sqlProvider.GetNotificationSubscriptions, query)).ToArray();
}