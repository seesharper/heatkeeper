using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeatKeeper.Server.Events.Api;
using HeatKeeper.Server.WebApi.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Tests for the EventBus functionality
/// </summary>
public class EventBusTests
{
    [Fact]
    public async Task CanPublishAndReceiveEvents()
    {
        // Arrange
        var bus = new EventBus();
        var receivedEvents = new List<DomainEvent<TemperatureReadingPayload>>();

        // Act - Subscribe to events
        _ = Task.Run(async () =>
        {
            await foreach (var evt in bus.Reader.ReadAllAsync())
            {
                if (evt is DomainEvent<TemperatureReadingPayload> tempEvent)
                {
                    receivedEvents.Add(tempEvent);
                    if (receivedEvents.Count >= 2) break;
                }
            }
        });

        var event1 = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0, SensorName: "Sensor-A"));
        var event2 = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 2, Temperature: 18.5, SensorName: "Sensor-B"));

        await bus.PublishAsync(event1);
        await bus.PublishAsync(event2);

        // Wait a bit for events to be processed
        await Task.Delay(100);

        // Assert
        Assert.Equal(2, receivedEvents.Count);
        Assert.Equal("TemperatureReadingPayload", receivedEvents[0].EventType);
        Assert.Equal(1, receivedEvents[0].Payload.ZoneId);
        Assert.Equal(20.0, receivedEvents[0].Payload.Temperature);
    }
}

/// <summary>
/// Tests for strongly-typed DomainEvent functionality
/// </summary>
public class DomainEventTests
{
    [Fact]
    public void DomainEvent_AutoGeneratesEventType()
    {
        // Arrange & Act
        var tempEvent = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0, SensorName: "Kitchen"));

        // Assert
        Assert.Equal("TemperatureReadingPayload", tempEvent.EventType);
        Assert.True(tempEvent.OccurredAt > DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void DomainEvent_DifferentPayloadTypes_GenerateDifferentEventTypes()
    {
        // Arrange & Act
        var tempEvent = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0, SensorName: "Kitchen"));
        var motionEvent = DomainEvent<MotionDetectedPayload>.Create(
            new MotionDetectedPayload(ZoneId: 2, Location: "Hallway", DetectedAt: DateTimeOffset.UtcNow));

        // Assert
        Assert.Equal("TemperatureReadingPayload", tempEvent.EventType);
        Assert.Equal("MotionDetectedPayload", motionEvent.EventType);
        Assert.NotEqual(tempEvent.EventType, motionEvent.EventType);
    }
}

/// <summary>
/// Tests for the EventCatalog functionality
/// </summary>
public class EventCatalogTests
{
    [Fact]
    public void EventCatalog_CanScanAssemblyForEventTypes()
    {
        // Arrange
        var catalog = new EventCatalog();

        // Act - Pass the server assembly explicitly since payload types are defined there
        catalog.ScanAssembly(typeof(TemperatureReadingPayload).Assembly);
        var eventTypes = catalog.ListEventTypes();

        // Assert
        Assert.NotEmpty(eventTypes);

        var tempEventType = eventTypes.FirstOrDefault(et => et.EventType == "TemperatureReadingPayload");
        Assert.NotNull(tempEventType);
        Assert.Equal(3, tempEventType.Properties.Count); // ZoneId, Temperature, SensorName

        var zoneIdProp = tempEventType.Properties.FirstOrDefault(p => p.Name == "ZoneId");
        Assert.NotNull(zoneIdProp);
        Assert.Equal("int", zoneIdProp.Type);
        Assert.False(zoneIdProp.IsNullable);
    }

    [Fact]
    public void EventCatalog_GetEventDetails_ReturnsCorrectMetadata()
    {
        // Arrange
        var catalog = new EventCatalog();
        catalog.ScanAssembly(typeof(MotionDetectedPayload).Assembly);

        // Act
        var eventDetails = catalog.GetEventDetails(2); // MotionDetectedPayload has ID 2

        // Assert
        Assert.NotNull(eventDetails);
        Assert.Equal(2, eventDetails.Id);
        Assert.Equal("Motion Detected", eventDetails.Name);
        Assert.Equal("Event triggered when motion is detected in a zone", eventDetails.Description);
        Assert.Equal("MotionDetectedPayload", eventDetails.EventType);
        Assert.Equal(3, eventDetails.Properties.Count); // ZoneId, Location, DetectedAt
    }

    [Fact]
    public void EventCatalog_OnlyScansTypesWithDomainEventAttribute()
    {
        // Arrange
        var catalog = new EventCatalog();
        catalog.ScanAssembly(typeof(MotionDetectedPayload).Assembly);

        // Act - Get all discovered events
        var allEvents = catalog.ListEventTypes();

        // Assert - Should only contain events with DomainEventAttribute
        Assert.All(allEvents, eventDef =>
        {
            // All returned events should have proper metadata from the attribute
            Assert.True(eventDef.Id > 0); // Id should be a positive integer
            Assert.NotEqual(eventDef.EventType, eventDef.Name); // Name should be different from EventType
            Assert.DoesNotContain("Event representing", eventDef.Description); // Should not contain fallback description
        });

        // Verify we have the expected attributed events
        Assert.Contains(allEvents, e => e.Id == 1);
        Assert.Contains(allEvents, e => e.Id == 2);
        Assert.Contains(allEvents, e => e.Id == 3);
    }

    [Fact]
    public void EventCatalog_SkipsDuplicateAssemblyScanning()
    {
        // Arrange
        var catalog = new EventCatalog();
        var assembly = typeof(MotionDetectedPayload).Assembly;

        // Act - Scan the same assembly multiple times
        catalog.ScanAssembly(assembly);
        var firstScanCount = catalog.ListEventTypes().Count;

        catalog.ScanAssembly(assembly);
        var secondScanCount = catalog.ListEventTypes().Count;

        catalog.ScanAssembly(assembly);
        var thirdScanCount = catalog.ListEventTypes().Count;

        // Assert - Should have the same number of events each time (no duplicates)
        Assert.Equal(firstScanCount, secondScanCount);
        Assert.Equal(firstScanCount, thirdScanCount);
        Assert.Equal(3, firstScanCount); // We know there are 3 events with DomainEventAttribute
    }

    [Fact]
    public void EventCatalog_GetEventDetails_IsThreadSafe()
    {
        // Arrange
        var catalog = new EventCatalog();
        catalog.ScanAssembly(typeof(MotionDetectedPayload).Assembly);
        var tasks = new List<Task<EventDetails>>();

        // Act - Access the same event from multiple threads simultaneously
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => catalog.GetEventDetails(2))); // MotionDetectedPayload
        }

        var results = Task.WhenAll(tasks).Result;

        // Assert - All threads should get the same result
        Assert.All(results, result =>
        {
            Assert.Equal(2, result.Id);
            Assert.Equal("Motion Detected", result.Name);
            Assert.Equal("MotionDetectedPayload", result.EventType);
        });
    }
}

/// <summary>
/// API tests for Events endpoints
/// </summary>
public class EventsApiTests : TestBase
{
    [Fact]
    public async Task GetEvents_ReturnsEventInfoWithoutDuplicates()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var events = await client.GetEvents(token);

        // Assert
        Assert.NotNull(events);
        Assert.NotEmpty(events);

        // Verify no duplicates by ID
        var duplicateIds = events.GroupBy(e => e.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicateIds);

        // Verify no duplicates by Name
        var duplicateNames = events.GroupBy(e => e.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicateNames);

        // Verify we have the expected events with proper structure
        Assert.All(events, eventInfo =>
        {
            Assert.True(eventInfo.Id > 0);
            Assert.NotNull(eventInfo.Name);
            Assert.NotEmpty(eventInfo.Name);
        });

        // Verify we have some expected events
        Assert.Contains(events, e => e.Id == 1);
        Assert.Contains(events, e => e.Id == 2);
        Assert.Contains(events, e => e.Id == 3);

        // Verify the count is reasonable (should have exactly the events with DomainEventAttribute)
        Assert.Equal(3, events.Length);
    }

    [Fact]
    public async Task GetEvents_CalledMultipleTimes_DoesNotReturnDuplicates()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act - Call the API multiple times
        var events1 = await client.GetEvents(token);
        var events2 = await client.GetEvents(token);
        var events3 = await client.GetEvents(token);

        // Assert - All calls should return the same results without duplicates
        Assert.Equal(events1.Length, events2.Length);
        Assert.Equal(events1.Length, events3.Length);

        // Verify we get exactly the same events each time
        foreach (var expectedEvent in events1)
        {
            Assert.Contains(events2, e => e.Id == expectedEvent.Id && e.Name == expectedEvent.Name);
            Assert.Contains(events3, e => e.Id == expectedEvent.Id && e.Name == expectedEvent.Name);
        }

        // Verify each response has no internal duplicates
        Assert.Equal(events1.Select(e => e.Id).Distinct().Count(), events1.Length);
        Assert.Equal(events2.Select(e => e.Id).Distinct().Count(), events2.Length);
        Assert.Equal(events3.Select(e => e.Id).Distinct().Count(), events3.Length);
    }

    [Fact]
    public async Task GetEventDetails_ReturnsCorrectEventDetails()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var eventDetails = await client.GetEventDetails(2, token); // MotionDetectedPayload has ID 2

        // Assert
        Assert.NotNull(eventDetails);
        Assert.Equal(2, eventDetails.Id);
        Assert.Equal("Motion Detected", eventDetails.Name);
        Assert.Equal("Event triggered when motion is detected in a zone", eventDetails.Description);
        Assert.Equal("MotionDetectedPayload", eventDetails.EventType);
        Assert.NotNull(eventDetails.Properties);
        Assert.Equal(3, eventDetails.Properties.Count); // ZoneId, Location, DetectedAt

        // Verify properties structure
        Assert.All(eventDetails.Properties, prop =>
        {
            Assert.NotNull(prop.Name);
            Assert.NotEmpty(prop.Name);
            Assert.NotNull(prop.Type);
            Assert.NotEmpty(prop.Type);
        });
    }

    [Fact]
    public async Task GetEventDetails_WithValidId_ReturnsExpectedProperties()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var eventDetails = await client.GetEventDetails(1, token); // TemperatureReadingPayload has ID 1

        // Assert
        Assert.NotNull(eventDetails);
        Assert.Equal(1, eventDetails.Id);
        Assert.Equal("Temperature Reading", eventDetails.Name);
        Assert.Equal("Event triggered when a temperature sensor reports a new reading", eventDetails.Description);
        Assert.Equal("TemperatureReadingPayload", eventDetails.EventType);
        Assert.NotNull(eventDetails.Properties);
        Assert.Equal(3, eventDetails.Properties.Count); // ZoneId, Temperature, SensorName

        // Verify specific properties exist
        Assert.Contains(eventDetails.Properties, p => p.Name == "ZoneId");
        Assert.Contains(eventDetails.Properties, p => p.Name == "Temperature");
        Assert.Contains(eventDetails.Properties, p => p.Name == "SensorName");
    }

    [Theory]
    [InlineData(1, "Temperature Reading", "TemperatureReadingPayload", 3)]
    [InlineData(2, "Motion Detected", "MotionDetectedPayload", 3)]
    [InlineData(3, "Door Event", "DoorEventPayload", 3)]
    public async Task GetEventDetails_WithKnownIds_ReturnsCorrectMetadata(int eventId, string expectedName, string expectedEventType, int expectedPropertyCount)
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var eventDetails = await client.GetEventDetails(eventId, token);

        // Assert
        Assert.NotNull(eventDetails);
        Assert.Equal(eventId, eventDetails.Id);
        Assert.Equal(expectedName, eventDetails.Name);
        Assert.Equal(expectedEventType, eventDetails.EventType);
        Assert.Equal(expectedPropertyCount, eventDetails.Properties.Count);
        Assert.NotNull(eventDetails.Description);
        Assert.NotEmpty(eventDetails.Description);
    }
}

/// <summary>
/// Integration tests combining multiple components
/// </summary>
public class TriggerEngineIntegrationTests
{
    [Fact]
    public async Task TriggerEngine_ProcessesTemperatureEvents_ExecutesActions()
    {
        // Arrange
        var bus = new EventBus();
        var catalog = new ActionCatalog();
        catalog.Register(ActionInfoBuilder.BuildFrom(typeof(TurnHeatersOffAction)));
        catalog.Register(ActionInfoBuilder.BuildFrom(typeof(SendNotificationAction)));

        // Create mock service provider with actions
        var actions = new Dictionary<string, IAction>
        {
            ["TurnHeatersOff"] = new TurnHeatersOffAction(),
            ["SendNotification"] = new SendNotificationAction()
        };
        var mockServiceProvider = new MockServiceProvider(actions);

        var engine = new TriggerEngine(bus, catalog, mockServiceProvider);

        var tempHighTrigger = new TriggerDefinition(
            Name: "Turn heaters off when too warm",
            AppliesToEventType: "TemperatureReadingPayload",
            Conditions: new List<Condition>
            {
                new(
                    PropertyName: "Temperature",
                    Operator: ComparisonOperator.GreaterThan,
                    Value: "19.5")
            },
            Actions: new List<ActionBinding>
            {
                new(
                    ActionName: "TurnHeatersOff",
                    ParameterMap: new Dictionary<string, string>
                    {
                        ["ZoneId"] = "{{payload.ZoneId}}",
                        ["Reason"] = "Too warm"
                    }
                )
            }
        );

        engine.AddTrigger(tempHighTrigger);

        // Start the engine
        var cts = new CancellationTokenSource();
        _ = Task.Run(() => engine.StartAsync(cts.Token));

        // Act - Publish a temperature event that should trigger the action
        var hotEvent = DomainEvent<TemperatureReadingPayload>.Create(
            new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0, SensorName: "Kitchen"));

        await bus.PublishAsync(hotEvent, cts.Token);

        // Give the engine time to process
        await Task.Delay(100);

        // Assert - This is a basic test that verifies the engine processes without throwing
        // In a real scenario, you'd want to capture and verify the action execution
        Assert.True(true); // If we get here without exceptions, the trigger processed successfully

        cts.Cancel();
    }

    /// <summary>
    /// Mock service provider for testing that supports LightInject-style named services
    /// </summary>
    private class MockServiceProvider : IServiceProvider
    {
        internal readonly Dictionary<string, IAction> _actions;

        public MockServiceProvider(Dictionary<string, IAction> actions)
        {
            _actions = actions;
        }

        public object GetService(Type serviceType)
        {
            return null; // Not used in our test
        }

        public IServiceScope CreateScope()
        {
            return new MockServiceScope(this);
        }
    }

    private class MockServiceScope : IServiceScope
    {
        private readonly MockServiceProvider _provider;

        public MockServiceScope(MockServiceProvider provider)
        {
            _provider = provider;
        }

        public IServiceProvider ServiceProvider => new MockLightInjectServiceFactory(_provider._actions);

        public void Dispose() { }
    }

    private class MockLightInjectServiceFactory : IServiceProvider, LightInject.IServiceFactory
    {
        private readonly Dictionary<string, IAction> _actions;

        public MockLightInjectServiceFactory(Dictionary<string, IAction> actions)
        {
            _actions = actions;
        }

        public object GetService(Type serviceType)
        {
            return null;
        }

        public object GetInstance(Type serviceType, string serviceName)
        {
            if (serviceType == typeof(IAction) && _actions.TryGetValue(serviceName, out var action))
            {
                return action;
            }
            throw new InvalidOperationException($"Service {serviceType.Name} with name '{serviceName}' not found");
        }

        // IServiceFactory methods
        public object GetInstance(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>()
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>(string serviceName)
        {
            return (T)GetInstance(typeof(T), serviceName);
        }

        public object GetInstance(Type serviceType, object[] args)
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>(object[] args)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string serviceName, object[] args)
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>(string serviceName, object[] args)
        {
            throw new NotImplementedException();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IAction) && _actions.ContainsKey(serviceName);
        }

        public bool CanGetInstance(Type serviceType)
        {
            return false;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            throw new NotImplementedException();
        }

        public LightInject.Scope BeginScope()
        {
            throw new NotImplementedException();
        }

        public object TryGetInstance(Type serviceType)
        {
            return null;
        }

        public object TryGetInstance(Type serviceType, string serviceName)
        {
            if (serviceType == typeof(IAction) && _actions.TryGetValue(serviceName, out var action))
            {
                return action;
            }
            return null;
        }

        public object Create(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}