namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record GetAllScheduledNotificationsQuery() : IQuery<ScheduledNotification[]>;

public record ScheduledNotification(long Id, NotificationType NotificationType, string CronExpression);

public class GetAllScheduledNotificationsQueryHandler(IDbConnection dbConnection) : IQueryHandler<GetAllScheduledNotificationsQuery, ScheduledNotification[]>
{
    public async Task<ScheduledNotification[]> HandleAsync(GetAllScheduledNotificationsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ScheduledNotification>("SELECT * FROM Notifications")).ToArray();
}

