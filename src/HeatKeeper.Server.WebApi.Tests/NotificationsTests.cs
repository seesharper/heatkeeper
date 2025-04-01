using System.Linq;
using HeatKeeper.Server.Notifications;
using HeatKeeper.Server.Users.Api;

namespace HeatKeeper.Server.WebApi.Tests;

public class NotificationsTests : TestBase
{
    [Fact]
    public async Task ShouldSubscribeToNotification()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var testUserId = await client.CreateUser(TestData.Users.StandardUser, testLocation.Token);
        var command = new PostNotificationCommand(testUserId, NotificationType.DeadSensors, "0 0 * * *", 12, "Test Notification", "Test Notification Description");
        await client.CreateNotification(command, testLocation.Token);
        var notificationSubscriptions = await client.GetNotifications(testUserId, testLocation.Token);
        var notificationSubscriptionDetails = await client.GetNotificationDetails(notificationSubscriptions.Single().Id, testLocation.Token);
        notificationSubscriptionDetails.NotificationType.Should().Be(NotificationType.DeadSensors);
        notificationSubscriptionDetails.HoursToSnooze.Should().Be(12);
        notificationSubscriptionDetails.Name.Should().Be("Test Notification");
        notificationSubscriptionDetails.Description.Should().Be("Test Notification Description");
    }
}

