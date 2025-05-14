using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.PushSubscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using WebPush;
using Xunit;
namespace HeatKeeper.Server.WebApi.Tests;

public class PushSubscriptionsTests : TestBase
{
    [Fact]
    public async Task ShouldCreatePushSubscription()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);

        
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        await client.Post(TestData.PushSubscriptions.TestPushSubscription);

        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>();
        var pushSubscription = (await queryExecutor.ExecuteAsync(new GetPushSubscriptionsByLocationQuery(testLocation.LocationId))).Single();
        pushSubscription.Endpoint.Should().Be(TestData.PushSubscriptions.Endpoint);
        pushSubscription.P256dh.Should().Be(TestData.PushSubscriptions.P256dh);
        pushSubscription.Auth.Should().Be(TestData.PushSubscriptions.Auth);
        pushSubscription.LastSeen.Should().Be(TestData.Clock.Today);

        now.SetUtcNow(TestData.Clock.LaterToday);

        await client.Post(TestData.PushSubscriptions.TestPushSubscription);

        pushSubscription = (await queryExecutor.ExecuteAsync(new GetPushSubscriptionsByLocationQuery(testLocation.LocationId))).Single();
        pushSubscription.Endpoint.Should().Be(TestData.PushSubscriptions.Endpoint);
        pushSubscription.P256dh.Should().Be(TestData.PushSubscriptions.P256dh);
        pushSubscription.Auth.Should().Be(TestData.PushSubscriptions.Auth);
        pushSubscription.LastSeen.Should().Be(TestData.Clock.LaterToday);
    }

    [Fact]
    public async Task ShouldSendPushNotificationWhenProgramChanges()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);

        var webPushClientMock = Factory.MockService<IWebPushClient>();
        
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        await client.Post(TestData.PushSubscriptions.TestPushSubscription);

        await client.UpdateProgram(TestData.Programs.UpdatedTestProgram(testLocation.NormalProgramId, testLocation.ScheduleId), testLocation.Token);
    }
}