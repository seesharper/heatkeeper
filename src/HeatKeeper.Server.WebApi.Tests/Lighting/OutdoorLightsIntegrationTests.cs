using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Lighting;
using HeatKeeper.Server.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

public class OutdoorLightsIntegrationTests : TestBase
{
    [Fact]
    public void OutdoorLightsController_ShouldBeRegistered_InDependencyContainer()
    {
        // Arrange & Act
        using var scope = Factory.Services.CreateScope();
        var controller = scope.ServiceProvider.GetService<IOutdoorLightsController>();

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeOfType<OutdoorLightsController>();
    }

    [Fact]
    public async Task OutdoorLightsController_ShouldPublishEvents_ThroughMessageBus()
    {
        // Arrange
        var receivedEvents = new List<OutdoorLightStateChanged>();

        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var controller = scope.ServiceProvider.GetRequiredService<IOutdoorLightsController>();

        // Subscribe to events
        messageBus.Subscribe<OutdoorLightStateChanged>((OutdoorLightStateChanged lightEvent) =>
        {
            receivedEvents.Add(lightEvent);
            return Task.CompletedTask;
        });

        // Act
        await controller.CheckAndPublishLightStates();

        // Process all messages
        await messageBus.ConsumeAllMessages<OutdoorLightStateChanged>();

        // Assert
        receivedEvents.Should().HaveCount(1);
        receivedEvents[0].State.Should().BeOneOf(LightState.On, LightState.Off);
        receivedEvents[0].Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        receivedEvents[0].Reason.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task OutdoorLightsController_ShouldDetectStateChanges()
    {
        // Arrange
        var receivedEvents = new List<OutdoorLightStateChanged>();

        // Configure for a specific time zone and use fake time provider
        Factory.ConfigureHostBuilder(hostBuilder =>
            hostBuilder.ConfigureServices((context, services) =>
            {
                var fakeTimeProvider = new FakeTimeProvider();
                services.AddSingleton<TimeProvider>(fakeTimeProvider);

                // Set initial time to night (2 AM)
                fakeTimeProvider.SetUtcNow(new DateTime(2024, 6, 21, 2, 0, 0, DateTimeKind.Utc));
            }));

        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var controller = scope.ServiceProvider.GetRequiredService<IOutdoorLightsController>();
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>() as FakeTimeProvider;

        messageBus.Subscribe<OutdoorLightStateChanged>((OutdoorLightStateChanged lightEvent) =>
        {
            receivedEvents.Add(lightEvent);
            return Task.CompletedTask;
        });

        // Act 1 - Initial state (night time)
        await controller.CheckAndPublishLightStates();

        // Act 2 - Change to day time
        timeProvider!.SetUtcNow(new DateTime(2024, 6, 21, 12, 0, 0, DateTimeKind.Utc));
        await controller.CheckAndPublishLightStates();

        // Process all messages at once
        await messageBus.ConsumeAllMessages<OutdoorLightStateChanged>();

        // Assert
        receivedEvents.Should().HaveCount(2);
        receivedEvents[0].State.Should().Be(LightState.On); // Night time
        receivedEvents[1].State.Should().Be(LightState.Off); // Day time
        receivedEvents[1].Reason.Should().Contain("changed from On to Off");
    }

    [Fact]
    public async Task GetCurrentLightState_ShouldWorkWithRealSunCalculations()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<IOutdoorLightsController>();

        // Act
        var states = await controller.GetCurrentLightStatesAsync();

        // Assert - Should return states for any locations in the database
        states.Should().NotBeNull();
        // If there are locations, their states should be either On or Off
        foreach (var locationState in states)
        {
            locationState.Value.Should().BeOneOf(LightState.On, LightState.Off);
        }
    }

    [Fact]
    public async Task MessageBus_ShouldDeliverLightingEvents_ToMultipleSubscribers()
    {
        // Arrange
        var subscriber1Events = new List<OutdoorLightStateChanged>();
        var subscriber2Events = new List<OutdoorLightStateChanged>();

        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var controller = scope.ServiceProvider.GetRequiredService<IOutdoorLightsController>();

        // Multiple subscribers
        messageBus.Subscribe<OutdoorLightStateChanged>((OutdoorLightStateChanged lightEvent) =>
        {
            subscriber1Events.Add(lightEvent);
            return Task.CompletedTask;
        });

        messageBus.Subscribe<OutdoorLightStateChanged>((OutdoorLightStateChanged lightEvent) =>
        {
            subscriber2Events.Add(lightEvent);
            return Task.CompletedTask;
        });

        // Act
        await controller.CheckAndPublishLightStates();
        await messageBus.ConsumeAllMessages<OutdoorLightStateChanged>();

        // Assert
        subscriber1Events.Should().HaveCount(1);
        subscriber2Events.Should().HaveCount(1);
        subscriber1Events[0].State.Should().Be(subscriber2Events[0].State);
    }
}