namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record GetSubscribedUsersQuery(NotificationType NotificationType) : IQuery<UserSubscribedToNotification[]>;

public class GetSubscribedUsersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetSubscribedUsersQuery, UserSubscribedToNotification[]>
{
    public async Task<UserSubscribedToNotification[]> HandleAsync(GetSubscribedUsersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UserSubscribedToNotification>(sqlProvider.GetUsersSubscribedToNotification, query)).ToArray();
}

public record UserSubscribedToNotification(long UserId, bool Enabled, long HoursToSnooze, DateTime LastSent);
