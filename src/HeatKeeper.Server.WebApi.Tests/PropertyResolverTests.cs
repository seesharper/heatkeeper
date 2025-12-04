using System;
using HeatKeeper.Server.Events;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Tests for PropertyResolver utility that handles {{propertyName}} placeholder resolution
/// </summary>
public class PropertyResolverTests
{
    [Fact]
    public void ResolveValue_WithNoPlaceholders_ReturnsOriginalValue()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0);
        var parameterValue = "literal value";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.Equal("literal value", result);
    }

    [Fact]
    public void ResolveValue_WithSinglePlaceholder_ReturnsTypedValue()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 42, Temperature: 25.0);
        var parameterValue = "{{ZoneId}}";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(42, result);
    }

    [Fact]
    public void ResolveValue_WithMultiplePlaceholders_ReturnsResolvedString()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 5, Temperature: 22.5, SensorName: "Kitchen");
        var parameterValue = "Zone {{ZoneId}} sensor {{SensorName}} reported {{Temperature}} degrees";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<string>(result);
        // Use culture-invariant comparison since ToString() can produce different decimal separators
        var resultString = result as string;
        Assert.NotNull(resultString);
        Assert.Contains("Zone 5", resultString);
        Assert.Contains("sensor Kitchen", resultString);
        Assert.Contains("reported", resultString);
        Assert.Contains("degrees", resultString);
    }

    [Fact]
    public void ResolveValue_WithMixedContentAndPlaceholder_ReturnsResolvedString()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 3, Temperature: 18.0);
        var parameterValue = "Temperature in zone {{ZoneId}} is too low";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Temperature in zone 3 is too low", result);
    }

    [Fact]
    public void ResolveValue_WithCaseInsensitivePropertyName_ResolvesCorrectly()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 7, Temperature: 20.0);
        var parameterValue = "{{zoneid}}"; // lowercase

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public void ResolveValue_WithNonExistentProperty_ReturnsNull()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0);
        var parameterValue = "{{NonExistentProperty}}";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResolveValue_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0);
        var parameterValue = "";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ResolveValue_WithStringProperty_ReturnsStringValue()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 25.0, SensorName: "Living Room");
        var parameterValue = "{{SensorName}}";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Living Room", result);
    }

    [Fact]
    public void ResolveValue_WithDoubleProperty_ReturnsDoubleValue()
    {
        // Arrange
        var payload = new TemperatureReadingPayload(ZoneId: 1, Temperature: 23.75);
        var parameterValue = "{{Temperature}}";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<double>(result);
        Assert.Equal(23.75, result);
    }

    [Fact]
    public void ResolveValue_WithComplexPayload_ResolvesCorrectly()
    {
        // Arrange
        var payload = new MotionDetectedPayload(
            LocationId: 1,
            ZoneId: 10,
            DetectedAt: new DateTimeOffset(2025, 12, 2, 10, 30, 0, TimeSpan.Zero)
        );
        var parameterValue = "Motion in Location {{LocationId}} (Zone {{ZoneId}})";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.Equal("Motion in Location 1 (Zone 10)", result);
    }

    [Fact]
    public void ResolveValue_WithDateTimeOffsetProperty_ReturnsDateTimeOffsetValue()
    {
        // Arrange
        var detectedAt = new DateTimeOffset(2025, 12, 2, 10, 30, 0, TimeSpan.Zero);
        var payload = new MotionDetectedPayload(LocationId: 1, ZoneId: 1, DetectedAt: detectedAt);
        var parameterValue = "{{DetectedAt}}";

        // Act
        var result = PropertyResolver.ResolveValue(parameterValue, payload);

        // Assert
        Assert.IsType<DateTimeOffset>(result);
        Assert.Equal(detectedAt, result);
    }
}
