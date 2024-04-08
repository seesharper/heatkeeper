using System;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using FluentAssertions;
using HeatKeeper.Server.PushSubscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Xunit;
namespace HeatKeeper.Server.WebApi.Tests;

public class PushSubscriptionsTests : TestBase
{
    [Fact]
    public async Task ShouldCreatePushSubscription()
    {
        var now = Factory.UseFakeTimeProvider(TestData.Clock.Today);

        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        await client.CreatePushSubscription(TestData.PushSubscriptions.TestPushSubscription, testLocation.Token);

        var queryExecutor = Factory.Services.GetRequiredService<IQueryExecutor>();
        var pushSubscription = (await queryExecutor.ExecuteAsync(new GetPushSubscriptionsByLocationQuery(testLocation.LocationId))).Single();
        pushSubscription.Endpoint.Should().Be(TestData.PushSubscriptions.Endpoint);
        pushSubscription.P256dh.Should().Be(TestData.PushSubscriptions.P256dh);
        pushSubscription.Auth.Should().Be(TestData.PushSubscriptions.Auth);
        pushSubscription.LastSeen.Should().Be(TestData.Clock.Today);

        now.SetUtcNow(TestData.Clock.LaterToday);

        await client.CreatePushSubscription(TestData.PushSubscriptions.TestPushSubscription, testLocation.Token);

        pushSubscription = (await queryExecutor.ExecuteAsync(new GetPushSubscriptionsByLocationQuery(testLocation.LocationId))).Single();
        pushSubscription.Endpoint.Should().Be(TestData.PushSubscriptions.Endpoint);
        pushSubscription.P256dh.Should().Be(TestData.PushSubscriptions.P256dh);
        pushSubscription.Auth.Should().Be(TestData.PushSubscriptions.Auth);
        pushSubscription.LastSeen.Should().Be(TestData.Clock.LaterToday);
    }
}