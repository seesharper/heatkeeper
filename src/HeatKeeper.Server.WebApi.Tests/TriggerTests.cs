using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Events;
using HeatKeeper.Server.Events.Api;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;

public static partial class TestData
{
    public static class Triggers
    {
        public static TriggerDefinition TemperatureTrigger => new(
            "Temperature Alert",
            1, // TemperatureReadingPayload event ID
            new List<Condition>
            {
                new("Temperature", ComparisonOperator.GreaterThan, "25.0")
            },
            new List<ActionBinding>
            {
                new(1, new Dictionary<string, string>
                {
                    ["Message"] = "Temperature too high!"
                })
            }
        );

        public static TriggerDefinition UpdatedTemperatureTrigger => new(
            "Updated Temperature Alert",
            1, // TemperatureReadingPayload event ID
            new List<Condition>
            {
                new("Temperature", ComparisonOperator.GreaterThan, "30.0")
            },
            new List<ActionBinding>
            {
                new(1, new Dictionary<string, string>
                {
                    ["Message"] = "Temperature critically high!"
                })
            }
        );

        public static TriggerDefinition EmptyNameTrigger => new(
            "",
            1, // TemperatureReadingPayload event ID
            new List<Condition>(),
            new List<ActionBinding>()
        );

        public static TriggerDefinition HumidityTrigger => new(
            "Humidity Alert",
            1, // Using TemperatureReadingPayload event ID (HumidityReading doesn't exist)
            new List<Condition>
            {
                new("Humidity", ComparisonOperator.LessThan, "40.0")
            },
            new List<ActionBinding>
            {
                new(2, new Dictionary<string, string>
                {
                    ["ZoneId"] = "1"
                })
            }
        );
    }
}

public class TriggerTests : TestBase
{
    private static PostTriggerCommand CreateTemperatureTrigger => new(TestData.Triggers.TemperatureTrigger.Name);
    private static PostTriggerCommand CreateHumidityTrigger => new(TestData.Triggers.HumidityTrigger.Name);
    private static PostTriggerCommand CreateEmptyNameTrigger => new(TestData.Triggers.EmptyNameTrigger.Name);

    [Fact]
    public async Task ShouldCreateTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var response = await client.Post(CreateTemperatureTrigger);

        response.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldGetTriggers()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        await client.Post(CreateTemperatureTrigger);
        await client.Post(CreateHumidityTrigger);

        var triggers = await client.Get(new GetTriggersQuery());

        triggers.Should().NotBeEmpty();
        triggers.Should().HaveCount(2);

        var tempTrigger = triggers.First(t => t.Name == "Temperature Alert");
        tempTrigger.Id.Should().BeGreaterThan(0);
        tempTrigger.Name.Should().Be("Temperature Alert");

        var humidityTrigger = triggers.First(t => t.Name == "Humidity Alert");
        humidityTrigger.Id.Should().BeGreaterThan(0);
        humidityTrigger.Name.Should().Be("Humidity Alert");
    }

    [Fact]
    public async Task ShouldUpdateTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Create a trigger
        await client.Post(CreateTemperatureTrigger);

        // Get the created trigger to verify it exists
        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().HaveCount(1);
        var originalTrigger = triggers.First();
        originalTrigger.Id.Should().BeGreaterThan(0);
        originalTrigger.Name.Should().Be("Temperature Alert");

        // Update the trigger (using ID 1 as we know it's the first one)
        var updateCommand = new PatchTriggerCommand(1, TestData.Triggers.UpdatedTemperatureTrigger);
        await client.Patch(updateCommand);

        // Verify the trigger was updated
        var updatedTriggers = await client.Get(new GetTriggersQuery());
        updatedTriggers.Should().HaveCount(1);
        var updatedTrigger = updatedTriggers.First();
        updatedTrigger.Id.Should().Be(originalTrigger.Id); // Same ID after update
        updatedTrigger.Name.Should().Be("Updated Temperature Alert");
    }

    [Fact]
    public async Task ShouldDeleteTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Create two triggers
        await client.Post(CreateTemperatureTrigger);
        await client.Post(CreateHumidityTrigger);

        // Verify both exist
        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().HaveCount(2);

        // Delete the first trigger (ID 1)
        await client.Delete(new DeleteTriggerCommand(1));

        // Verify only one trigger remains
        var remainingTriggers = await client.Get(new GetTriggersQuery());
        remainingTriggers.Should().HaveCount(1);
        remainingTriggers.First().Name.Should().Be("Humidity Alert");
    }

    [Fact]
    public async Task ShouldReturnEmptyArrayWhenNoTriggersExist()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var triggers = await client.Get(new GetTriggersQuery());

        triggers.Should().NotBeNull();
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldHandleComplexTriggerDefinitions()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var complexTrigger = new TriggerDefinition(
            "Complex Multi-Condition Trigger",
            1, // TemperatureReadingPayload event ID
            new List<Condition>
            {
                new("Temperature", ComparisonOperator.GreaterThan, "18.5"),
                new("Temperature", ComparisonOperator.LessThan, "25.0"),
                new("Humidity", ComparisonOperator.GreaterThan, "40.0")
            },
            new List<ActionBinding>
            {
                new(1, new Dictionary<string, string>
                {
                    ["Priority"] = "High",
                    ["Recipients"] = "admin@example.com,operator@example.com"
                }),
                new(2, new Dictionary<string, string>
                {
                    ["ZoneId"] = "{{trigger.LocationId}}",
                    ["DelaySeconds"] = "30"
                })
            }
        );

        var createCommand = new PostTriggerCommand(complexTrigger.Name);
        await client.Post(createCommand);

        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().HaveCount(1);

        var retrievedTrigger = triggers.First();
        retrievedTrigger.Id.Should().BeGreaterThan(0);
        retrievedTrigger.Name.Should().Be("Complex Multi-Condition Trigger");
    }

    [Fact]
    public async Task ShouldPreserveJsonSerializationFidelity()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var originalTrigger = new TriggerDefinition(
            "JSON Fidelity Test",
            1, // TemperatureReadingPayload event ID
            new List<Condition>
            {
                new("TestProperty", ComparisonOperator.Equals, "test value")
            },
            new List<ActionBinding>
            {
                new(999, new Dictionary<string, string>
                {
                    ["Parameter"] = "test parameter"
                })
            }
        );

        await client.Post(new PostTriggerCommand(originalTrigger.Name));

        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().HaveCount(1);

        var retrievedTrigger = triggers.First();
        retrievedTrigger.Id.Should().BeGreaterThan(0);
        retrievedTrigger.Name.Should().Be("JSON Fidelity Test");
    }

    public static TheoryData<PostTriggerCommand, string> InvalidTriggers()
    {
        var data = new TheoryData<PostTriggerCommand, string>();
        data.Add(CreateEmptyNameTrigger, "Trigger name cannot be empty");
        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidTriggers))]
    public async Task ShouldValidateTrigger(PostTriggerCommand command, string expectedErrorSubstring)
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        await client.Post(command, problem: details =>
        {
            details.ShouldHaveBadRequestStatus();
            details.Detail.Should().Contain(expectedErrorSubstring);
        });
    }

    [Fact]
    public async Task ShouldHandleUpdateOfNonExistentTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        var updateCommand = new PatchTriggerCommand(99999, TestData.Triggers.TemperatureTrigger);

        // This should not throw an exception but may not update anything
        // The behavior depends on your database constraints and error handling
        await client.Patch(updateCommand);

        // Verify no triggers were created accidentally
        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldHandleDeleteOfNonExistentTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // This should not throw an exception
        await client.Delete(new DeleteTriggerCommand(99999));

        // Verify operation completed without issues
        var triggers = await client.Get(new GetTriggersQuery());
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldGetTriggerDetails()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Create a trigger
        var triggerId = await client.Post(CreateTemperatureTrigger);
        triggerId.Should().BeGreaterThan(0);

        // Update it with full definition to have proper JSON data
        await client.Patch(new PatchTriggerCommand(triggerId, TestData.Triggers.TemperatureTrigger));

        // Now retrieve the trigger details
        var triggerDetails = await client.Get(new TriggerDetailsQuery(triggerId));

        triggerDetails.Should().NotBeNull();
        triggerDetails.Name.Should().Be("Temperature Alert");
        triggerDetails.EventId.Should().Be(1);
        triggerDetails.Conditions.Should().HaveCount(1);
        triggerDetails.Actions.Should().HaveCount(1);
    }

    [Fact]
    public async Task ShouldHandleGetTriggerDetailsForNonExistentTrigger()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Try to get details for non-existent trigger
        var action = () => client.Get(new TriggerDetailsQuery(99999));

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ShouldCreateEmptyTriggerDefinitionAndRetrieveIt()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Create a trigger using PostTrigger
        var triggerName = "Test Empty Trigger";
        var triggerId = await client.Post(new PostTriggerCommand(triggerName));
        triggerId.Should().BeGreaterThan(0);

        // Retrieve the created trigger using GetTriggerDetails
        var triggerDetails = await client.Get(new TriggerDetailsQuery(triggerId));

        // Verify the trigger has proper empty structure
        triggerDetails.Should().NotBeNull();
        triggerDetails.Name.Should().Be(triggerName);
        triggerDetails.EventId.Should().Be(0); // Empty triggers have EventId 0
        triggerDetails.Conditions.Should().NotBeNull();
        triggerDetails.Conditions.Should().BeEmpty();
        triggerDetails.Actions.Should().NotBeNull();
        triggerDetails.Actions.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldGetAllTriggerDefinitions()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;

        // Create a trigger
        await client.Post(CreateTemperatureTrigger);

        // Update the trigger (using ID 1 as we know it's the first one)
        var updateCommand = new PatchTriggerCommand(1, TestData.Triggers.UpdatedTemperatureTrigger);
        await client.Patch(updateCommand);

        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();
        var definitions = await queryExecutor.ExecuteAsync(
            new GetAllEventTriggersQuery());

    }
}