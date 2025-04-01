namespace HeatKeeper.Server.Notifications.Api;

[RequireUserRole]
[Get("/api/notifications/{notificationId}")]
public record GetNotificationDetailsQuery(long NotificationId) : IQuery<NotificationDetails>;

public record NotificationDetails(long Id, string Name, string Description, bool Enabled, DateTime LastSent, NotificationType NotificationType, string CronExpression, long HoursToSnooze);

public class GetNotificationDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationDetailsQuery, NotificationDetails>
{
    public async Task<NotificationDetails> HandleAsync(GetNotificationDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationDetails>(sqlProvider.GetNotificationDetails, query)).Single();
}