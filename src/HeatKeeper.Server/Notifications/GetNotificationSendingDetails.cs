namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record GetNotificationSendingDetailsQuery(long NotificationId) : IQuery<NotificationSendingDetails>;

public record NotificationSendingDetails(bool Enabled, DateTime LastSent, long HoursToSnooze);

public class GetNotificationSendingDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetNotificationSendingDetailsQuery, NotificationSendingDetails>
{
    public async Task<NotificationSendingDetails> HandleAsync(GetNotificationSendingDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<NotificationSendingDetails>(sqlProvider.GetNotificationSendingDetails, query)).Single();
}