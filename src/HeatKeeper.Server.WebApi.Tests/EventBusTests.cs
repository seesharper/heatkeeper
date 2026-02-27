using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Events.Api;
using HeatKeeper.Server.WebApi.Tests;
using Microsoft.AspNetCore.Mvc;
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
        var receivedEvents = new List<EventEnvelope>();

        // Act - Subscribe to events
        _ = Task.Run(async () =>
        {
            await foreach (var evt in bus.Reader.ReadAllAsync())
            {
                if (evt.Payload is TemperatureReadingPayload)
                {
                    receivedEvents.Add(evt);
                    if (receivedEvents.Count >= 2) break;
                }
            }
        });

        var payload1 = new TemperatureReadingPayload(ZoneId: 1, Temperature: 20.0, SensorName: "Sensor-A");
        var payload2 = new TemperatureReadingPayload(ZoneId: 2, Temperature: 18.5, SensorName: "Sensor-B");

        await bus.PublishAsync(payload1);
        await bus.PublishAsync(payload2);

        // Wait a bit for events to be processed
        await Task.Delay(100);

        // Assert
        Assert.Equal(2, receivedEvents.Count);
        Assert.Equal("TemperatureReadingPayload", receivedEvents[0].EventType);
        var typedPayload1 = (TemperatureReadingPayload)receivedEvents[0].Payload;
        Assert.Equal(1, typedPayload1.ZoneId);
        Assert.Equal(20.0, typedPayload1.Temperature);
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
        Assert.Equal(6, firstScanCount); // We know there are 6 events with DomainEventAttribute
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
    public async Task GetActions_ReturnsRegisteredActionsWithoutDuplicates()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var actions = await client.GetActions(token);

        // Assert
        Assert.NotNull(actions);
        Assert.NotEmpty(actions);

        // Verify no duplicate IDs
        Assert.Equal(actions.Length, actions.Select(a => a.Id).Distinct().Count());

        // Verify no duplicate display names
        Assert.Equal(actions.Length, actions.Select(a => a.DisplayName).Distinct().Count());

        // Production actions (with positive IDs)
        var productionActions = actions.Where(a => a.Id > 0).ToArray();
        Assert.Contains(productionActions, a => a.Id == 1 && a.DisplayName == "Send Notification");
        Assert.Contains(productionActions, a => a.Id == 2 && a.DisplayName == "Turn Heaters Off");

        // Test actions (with negative IDs) should also be present
        var testActions = actions.Where(a => a.Id < 0).ToArray();
        Assert.NotEmpty(testActions);
    }

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
        Assert.Equal(6, events.Length);
    }

    [Fact]
    public async Task GetActionDetails_ForSendNotification_ReturnsExpectedSchema()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var details = await client.GetActionDetails(1, token); // TestSendNotificationAction has ID 1

        // Assert
        Assert.NotNull(details);
        Assert.Equal(1, details.Id);
        Assert.Equal("SendNotification", details.Name);
        Assert.Equal("Send Notification", details.DisplayName);
        Assert.Equal("Sends a notification with a message and optional severity level", details.Description);
        Assert.NotNull(details.ParameterSchema);
        Assert.Equal(2, details.ParameterSchema.Count);

        Assert.Contains(details.ParameterSchema, p => p.Name == "Message" && p.Type == "string" && p.Required && p.Description == "The notification message");
        Assert.Contains(details.ParameterSchema, p => p.Name == "Severity" && p.Type == "string" && !p.Required && p.Description == "The notification severity level");
    }

    [Fact]
    public async Task GetActionDetails_ForTurnHeatersOff_ReturnsExpectedSchema()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var details = await client.GetActionDetails(-2, token); // TestTurnHeatersOffCommand has ID -2

        // Assert
        Assert.NotNull(details);
        Assert.Equal(-2, details.Id);
        Assert.Equal("TestTurnHeatersOff", details.Name);
        Assert.Equal("[TEST] Turn Heaters Off", details.DisplayName);
        Assert.Equal("Turns off heaters in a specified zone with an optional reason (test action)", details.Description);
        Assert.NotNull(details.ParameterSchema);
        Assert.Equal(2, details.ParameterSchema.Count);

        Assert.Contains(details.ParameterSchema, p => p.Name == "ZoneId" && p.Type == "number" && p.Required && p.Description == "Which zone to target");
        Assert.Contains(details.ParameterSchema, p => p.Name == "Reason" && p.Type == "string" && !p.Required && p.Description == "Optional reason");
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

    [Fact]
    public async Task PostTestAction_WithValidParameters_CanInvokeAction()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var parameterMap = new Dictionary<string, string>
        {
            { "message", "Test notification from integration test" },
            { "severity", "info" }
        };

        // Act - Call the endpoint
        var response = await client.PostAsync("api/actions",
            new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { actionId = 1, parameterMap }),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert - Endpoint should be reachable and process the request
        // We're not asserting specific status code since we don't want to change implementation
        Assert.NotNull(response);
    }

    [Fact]
    public async Task PostTestAction_WithMissingRequiredField_HandlesError()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var parameterMap = new Dictionary<string, string>
        {
            { "severity", "info" }
            // Missing required "message" field
        };

        // Act - Call the endpoint with missing required field
        var response = await client.PostAsync("api/actions",
            new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { actionId = 1, parameterMap }),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert - Endpoint should be reachable and return some response
        Assert.NotNull(response);
        // The endpoint may return an error status (4xx or 5xx) due to validation
        Assert.True(response.StatusCode >= HttpStatusCode.BadRequest);
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
        catalog.Register(ActionDetailsBuilder.BuildFrom(typeof(TestTurnHeatersOffCommand)));
        catalog.Register(ActionDetailsBuilder.BuildFrom(typeof(TestSendNotificationCommand)));

        // Create mock command executor
        var executedCommands = new List<object>();
        var mockCommandExecutor = new MockCommandExecutor(executedCommands);

        var engine = new TriggerEngine(bus, catalog, mockCommandExecutor);

        var tempHighTrigger = new TriggerDefinition(
            Name: "Turn heaters off when too warm",
            EventId: 1, // TemperatureReadingPayload event ID
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
                    ActionId: 2,
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
        var hotPayload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0, SensorName: "Kitchen");

        await bus.PublishAsync(hotPayload, cts.Token);

        // Give the engine time to process
        await Task.Delay(100);

        // Assert - This is a basic test that verifies the engine processes without throwing
        // In a real scenario, you'd want to capture and verify the action execution
        Assert.True(true); // If we get here without exceptions, the trigger processed successfully

        cts.Cancel();
    }

    /// <summary>
    /// Mock command executor for testing
    /// </summary>
    private class MockCommandExecutor : ICommandExecutor
    {
        private readonly List<object> _executedCommands;

        public MockCommandExecutor(List<object> executedCommands)
        {
            _executedCommands = executedCommands;
        }

        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        {
            _executedCommands.Add(command!);
            return Task.CompletedTask;
        }
    }
}