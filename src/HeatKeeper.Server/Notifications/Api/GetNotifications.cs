namespace HeatKeeper.Server.Notifications.Api;


[RequireUserRole]
[Get("api/notifications")]
public record GetNotificationsQuery : IQuery<NotificationInfo[]>;

public class GetLocations(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationsQuery, NotificationInfo[]>
{
    public async Task<NotificationInfo[]> HandleAsync(GetNotificationsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationInfo>(sqlProvider.GetNotifications, query)).ToArray();
}

public record NotificationInfo(long Id, string Name);