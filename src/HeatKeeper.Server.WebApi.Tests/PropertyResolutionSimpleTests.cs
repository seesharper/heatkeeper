#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Events;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Simplified test to verify property resolution functionality
/// </summary>
public class PropertyResolutionSimpleTests
{
    private readonly ITestOutputHelper _output;

    public PropertyResolutionSimpleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void PropertyResolver_DirectTest_WorksCorrectly()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 42, Temperature: 25.5, SensorName: "Kitchen");

        // Act
        var zoneIdResult = PropertyResolver.ResolveValue("{{ZoneId}}", payload);
        var mixedResult = PropertyResolver.ResolveValue("Zone {{ZoneId}} temp {{Temperature}}", payload);
        var literalResult = PropertyResolver.ResolveValue("literal value", payload);

        // Assert
        Assert.Equal(42, zoneIdResult);
        Assert.Contains("Zone 42", mixedResult as string);
        Assert.Equal("literal value", literalResult);

        _output.WriteLine($"ZoneId resolved to: {zoneIdResult}");
        _output.WriteLine($"Mixed resolved to: {mixedResult}");
        _output.WriteLine($"Literal resolved to: {literalResult}");
    }

    [Fact]
    public void TriggerEngine_ManualInvokeAction_WithResolvedParameters()
    {
        // This tests that our PropertyResolver works when parameters are passed to an action
        // without going through the full event bus flow

        // Arrange
        var catalog = new ActionCatalog();
        catalog.Register(ActionDetailsBuilder.BuildFrom(typeof(TestDisableHeaterAction)));

        // Simulate what happens in TriggerEngine.ResolveParameters
        var eventPayload = new TestEventPayload(HeaterId: 99);
        var parameterMap = new Dictionary<string, string>
        {
            ["HeaterId"] = "{{HeaterId}}"
        };

        // Resolve parameters like TriggerEngine does
        var resolvedParams = new Dictionary<string, object?>();
        foreach (var kvp in parameterMap)
        {
            resolvedParams[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, eventPayload);
        }

        _output.WriteLine($"Resolved HeaterId parameter: {resolvedParams["HeaterId"]} (type: {resolvedParams["HeaterId"]?.GetType().Name})");

        // Verify the parameter was resolved to the correct type
        Assert.Equal(99L, Convert.ToInt64(resolvedParams["HeaterId"]));
    }

    [DomainEvent(999, "Test Event", "Test event for property resolution")]
    public record TestEventPayload(long HeaterId);
}
