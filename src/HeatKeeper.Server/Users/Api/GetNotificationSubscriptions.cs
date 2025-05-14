namespace HeatKeeper.Server.Users.Api;

[Get("/api/notification-subscriptions")]
[RequireUserRole]
public record GetNotificationSubscriptionsQuery() : IQuery<NotificationSubscriptionInfo[]>;

public record NotificationSubscriptionInfo(long Id, string Name, bool IsSubscribed);

public class GetNotificationSubscriptions(IUserContext userContext, IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationSubscriptionsQuery, NotificationSubscriptionInfo[]>
{
    public async Task<NotificationSubscriptionInfo[]> HandleAsync(GetNotificationSubscriptionsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationSubscriptionInfo>(sqlProvider.GetNotificationSubscriptions, new { UserId = userContext.Id })).ToArray();
}