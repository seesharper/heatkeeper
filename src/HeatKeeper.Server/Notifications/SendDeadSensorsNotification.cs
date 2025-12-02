using System.Text;
using HeatKeeper.Server.Messaging;
using HeatKeeper.Server.PushSubscriptions;
using HeatKeeper.Server.Sensors.Api;
using WebPush;

namespace HeatKeeper.Server.Notifications;

[RequireBackgroundRole]
public record SendDeadSensorsNotificationCommand(long NotificationId);

public class SendDeadSensorsNotification(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, IMessageBus messageBus) : ICommandHandler<SendDeadSensorsNotificationCommand>
{
    public async Task HandleAsync(SendDeadSensorsNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var deadSensors = await queryExecutor.ExecuteAsync(new DeadSensorsQuery(), cancellationToken);
        if (deadSensors.Length == 0)
        {
            // No dead sensors, nothing to do
            return;
        }

        var payLoad = CreateNotificationPayload(deadSensors);
        var notificationSendingDetails = await queryExecutor.ExecuteAsync(new GetNotificationSendingDetailsQuery(command.NotificationId), cancellationToken);


        var usersSubscribedToNotification = await queryExecutor.ExecuteAsync(new GetSubscribedUsersQuery(command.NotificationId), cancellationToken);
        foreach (var userSubscribedToNotification in usersSubscribedToNotification)
        {
            if (notificationSendingDetails.LastSent.AddHours(notificationSendingDetails.HoursToSnooze) > DateTime.UtcNow)
            {
                continue;
            }


            var pushSubscriptionByUser = await queryExecutor.ExecuteAsync(new GetPushSubscriptionByUserQuery(userSubscribedToNotification.UserId), cancellationToken);
            if (pushSubscriptionByUser.Length == 0)
            {
                continue;
            }
            foreach (var pushSubscriptionDetails in pushSubscriptionByUser)
            {
                var pushSubscription = new PushSubscription(pushSubscriptionDetails.Endpoint, pushSubscriptionDetails.P256dh, pushSubscriptionDetails.Auth);
                await messageBus.Publish(new SendPushNotificationCommand(pushSubscription, payLoad.ToString()));
            }
        }
    }

    private static StringBuilder CreateNotificationPayload(DeadSensor[] deadSensors)
    {
        StringBuilder payLoad = new StringBuilder();
        foreach (var deadSensor in deadSensors)
        {
            var message = $"Sensor '{deadSensor.Name}' in zone '{deadSensor.Zone}' at location '{deadSensor.Location}' has not been seen since {deadSensor.LastSeen}.";
            payLoad.AppendLine(message);
        }

        return payLoad;
    }
}