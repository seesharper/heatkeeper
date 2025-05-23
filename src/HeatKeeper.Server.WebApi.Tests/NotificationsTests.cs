using System;
using System.Linq;
using System.Threading;
using CQRS.AspNet.Testing;
using CsvHelper;
using HeatKeeper.Server.Messaging;
using HeatKeeper.Server.Notifications;
using HeatKeeper.Server.Notifications.Api;
using HeatKeeper.Server.PushSubscriptions;
using HeatKeeper.Server.Users.Api;
using Janitor;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebPush;

namespace HeatKeeper.Server.WebApi.Tests;

public static partial class TestData
{
    public static class Notifications
    {
        public static PostNotificationCommand DeadSensorNotification => new(
            "Test Notification",
            "Test Notification Description",
            NotificationType.DeadSensors,
            "0 0 * * *",
            12
        );
        public static PostNotificationCommand InvalidCronNotification => new(
            "Test Notification",
            "Test Notification Description",
            NotificationType.DeadSensors,
            "0 0 * * * *",
            12
        );
        public static PostNotificationCommand EmptyNameNotification => new(
            "",
            "Test Notification Description",
            NotificationType.DeadSensors,
            "0 0 * * *",
            12
        );

        public static PatchNotificationCommand UpdatedDeadSensorNotification => new(0, "Updated Test Notification", "Updated Test Notification Description", NotificationType.DeadSensors, "* * * * *", 24);
    }
}
public class NotificationsTests : TestBase
{
    private static PostNotificationCommand DeadSensorNotification => new("Test Notification",
        "Test Notification Description",
        NotificationType.DeadSensors,
        "0 0 * * *",
        12);
    private static PatchNotificationCommand UpdatedDeadSensorNotification => new(
        0,
        "Updated Test Notification",
        "Updated Test Notification Description",
        NotificationType.DeadSensors,
        "* * * * *",
        24);

    [Fact]
    public async Task ShouldCreateNotification()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var notificationId = await client.Post(DeadSensorNotification);
        var notificationDetails = await client.Get(new NotificationDetailsQuery(notificationId));

        notificationDetails.Name.Should().Be(DeadSensorNotification.Name);
        notificationDetails.Description.Should().Be(DeadSensorNotification.Description);
        notificationDetails.CronExpression.Should().Be(DeadSensorNotification.CronExpression);
        notificationDetails.NotificationType.Should().Be(DeadSensorNotification.NotificationType);
        notificationDetails.HoursToSnooze.Should().Be(DeadSensorNotification.HoursToSnooze);
    }

    [Fact]
    public async Task ShouldUpdateNotification()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var notificationId = await client.Post(DeadSensorNotification);
        await client.Patch(UpdatedDeadSensorNotification with { Id = notificationId });
        var notificationDetails = await client.Get(new NotificationDetailsQuery(notificationId));

        notificationDetails.Name.Should().Be(UpdatedDeadSensorNotification.Name);
        notificationDetails.Description.Should().Be(UpdatedDeadSensorNotification.Description);
        notificationDetails.CronExpression.Should().Be(UpdatedDeadSensorNotification.CronExpression);
        notificationDetails.HoursToSnooze.Should().Be(UpdatedDeadSensorNotification.HoursToSnooze);
        notificationDetails.NotificationType.Should().Be(UpdatedDeadSensorNotification.NotificationType);
    }

    [Fact]
    public async Task ShouldGetNotifications()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        await client.Post(DeadSensorNotification);

        var notifications = await client.Get(new GetNotificationsQuery());

        notifications.Should().NotBeEmpty();
        notifications.Should().HaveCount(1);
        notifications.First().Name.Should().Be(DeadSensorNotification.Name);
    }

    [Fact]
    public async Task ShouldDeleteNotification()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var notificationId = await client.Post(DeadSensorNotification);

        await client.Delete(new DeleteNotificationCommand(notificationId));

        var notifications = await client.Get(new GetNotificationsQuery());
        notifications.Should().BeEmpty();
    }

    public static TheoryData<PostNotificationCommand, string> InvalidNotifications()
    {
        var data = new TheoryData<PostNotificationCommand, string>();
        data.Add(DeadSensorNotification with { Name = "" }, "Name cannot be null or empty.");
        data.Add(DeadSensorNotification with { CronExpression = "InvalidCronExpression" }, $"The cron expression InvalidCronExpression is not valid for notification {TestData.Notifications.InvalidCronNotification.Name}");
        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidNotifications))]
    public async Task ShouldValidateNotification(PostNotificationCommand command, string errorMessage)
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        await client.Post(command, problem: details =>
        {
            details.ShouldHaveBadRequestStatus();
            details.Detail.Should().Be(errorMessage);
        });
    }

    [Fact]
    public async Task ShouldGetNotificationsBeingSubscribedTo()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var notificationId = await client.Post(DeadSensorNotification);
        await client.Post(new PostNotificationSubscriptionCommand(notificationId));

        var notificationSubscriptions = await client.Get(new GetNotificationSubscriptionsQuery());

        notificationSubscriptions.Length.Should().Be(1);
        notificationSubscriptions.First().IsSubscribed.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldGetNotificationsNotBeingSubscribedTo()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var notificationId = await client.Post(DeadSensorNotification);

        var notificationSubscriptions = await client.Get(new GetNotificationSubscriptionsQuery());

        notificationSubscriptions.Length.Should().Be(1);
        notificationSubscriptions.First().IsSubscribed.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldDeleteNotificationSubscription()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var notificationId = await client.Post(DeadSensorNotification);
       
       
       
       var notificationSubscriptionId = await client.Post(new PostNotificationSubscriptionCommand(notificationId));
        await client.Delete(new DeleteNotificationSubscription(notificationSubscriptionId));
        var notificationSubscriptions = await client.Get(new GetNotificationSubscriptionsQuery());
        notificationSubscriptions.Length.Should().Be(1);
        notificationSubscriptions.First().IsSubscribed.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldGetNotificationTypes()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var notificationTypes = await client.Get(new GetNotificationTypesQuery());

        notificationTypes.Should().NotBeEmpty();
        notificationTypes.Should().HaveCount(4);        
    }


    [Fact]
    public async Task ShouldSendDeadSensorNotification()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var webPushClientMock = Factory.MockService<IWebPushClient>();
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var janitor = Factory.Services.GetRequiredService<IJanitor>();
        var testUserId = await client.Post(TestData.Users.StandardUser);
        var notificationId = await client.Post(DeadSensorNotification);
        await client.Post(new PostNotificationSubscriptionCommand(notificationId));
        await client.Post(TestData.PushSubscriptions.TestPushSubscription);

        now.Advance(TimeSpan.FromHours(14));

        await janitor.Run($"ScheduledNotification_{notificationId}");
        var messageBus = Factory.Services.GetRequiredService<IMessageBus>();
        await messageBus.ConsumeAllMessages<SendPushNotificationCommand>();


        webPushClientMock.Verify(m => m.SendNotificationAsync(It.IsAny<PushSubscription>(), It.IsAny<string>(), It.IsAny<VapidDetails>(), It.IsAny<CancellationToken>()), Times.Once);

    }


}

