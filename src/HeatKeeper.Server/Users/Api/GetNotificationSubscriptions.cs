namespace HeatKeeper.Server.Users.Api;

[Get("/api/users/{userId}/notifications")]
[RequireUserRole]
public record GetNotificationsQuery(long UserId) : IQuery<NotificationInfo[]>;

public record NotificationInfo(long Id, string Name, bool Enabled);

public class GetNotifications(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationsQuery, NotificationInfo[]>
{
    public async Task<NotificationInfo[]> HandleAsync(GetNotificationsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationInfo>(sqlProvider.GetNotifications, query)).ToArray();
}