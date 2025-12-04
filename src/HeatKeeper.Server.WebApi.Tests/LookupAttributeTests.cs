using System.Linq;
using HeatKeeper.Server.Events;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Tests for the Lookup attribute feature
/// </summary>
public class LookupAttributeTests
{
    [Fact]
    public void EventPropertyInfo_WithLookupAttribute_IncludesLookupUrl()
    {
        // Arrange
        var catalog = new EventCatalog();
        catalog.ScanAssembly(typeof(TemperatureReadingPayload).Assembly);

        // Act
        var eventDetails = catalog.GetEventDetails(2); // MotionDetectedPayload has ID 2
        var zoneIdProperty = eventDetails.Properties.FirstOrDefault(p => p.Name == "ZoneId");

        // Assert
        Assert.NotNull(zoneIdProperty);
        Assert.Equal("api/locations/{locationId}/zones", zoneIdProperty.LookupUrl);
    }

    [Fact]
    public void ActionParameter_WithLookupAttribute_IncludesLookupUrl()
    {
        // Arrange - TestTurnHeatersOffCommand has ZoneId with Lookup attribute
        var actionDetails = ActionDetailsBuilder.BuildFrom(typeof(TestTurnHeatersOffCommand));

        // Act
        var zoneIdParameter = actionDetails.ParameterSchema.FirstOrDefault(p => p.Name == "ZoneId");

        // Assert
        Assert.NotNull(zoneIdParameter);
        Assert.Equal("api/zones", zoneIdParameter.LookupUrl);
    }

    [Fact]
    public void EventPropertyInfo_WithoutLookupAttribute_HasNullLookupUrl()
    {
        // Arrange
        var catalog = new EventCatalog();
        catalog.ScanAssembly(typeof(TemperatureReadingPayload).Assembly);

        // Act
        var eventDetails = catalog.GetEventDetails(1); // TemperatureReadingPayload
        var temperatureProperty = eventDetails.Properties.FirstOrDefault(p => p.Name == "Temperature");

        // Assert
        Assert.NotNull(temperatureProperty);
        Assert.Null(temperatureProperty.LookupUrl);
    }

    [Fact]
    public void ActionParameter_WithoutLookupAttribute_HasNullLookupUrl()
    {
        // Arrange
        var actionDetails = ActionDetailsBuilder.BuildFrom(typeof(TestTurnHeatersOffCommand));

        // Act
        var reasonParameter = actionDetails.ParameterSchema.FirstOrDefault(p => p.Name == "Reason");

        // Assert
        Assert.NotNull(reasonParameter);
        Assert.Null(reasonParameter.LookupUrl);
    }
}
