namespace HeatKeeper.Server.Notifications.Api;

[RequireUserRole]
[Get("/api/notifications/{notificationId}")]
public record NotificationDetailsQuery(long NotificationId) : IQuery<NotificationDetails>;

public record NotificationDetails(long Id, string Name, string Description, DateTime LastSent, NotificationType NotificationType, string CronExpression, long HoursToSnooze);

public class GetNotificationDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<NotificationDetailsQuery, NotificationDetails>
{
    public async Task<NotificationDetails> HandleAsync(NotificationDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationDetails>(sqlProvider.GetNotificationDetails, query)).Single();
}