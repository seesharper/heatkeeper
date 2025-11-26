using System;
using System.Collections.Generic;
using FluentAssertions;
using HeatKeeper.Server.Lighting;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

public class OutdoorLightsConfigurationTests
{
    [Fact]
    public void GetOutdoorLightsLatitude_ShouldReturnConfiguredValue()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "51.5074"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var latitude = config.GetOutdoorLightsLatitude();

        // Assert
        latitude.Should().Be(51.5074);
    }

    [Fact]
    public void GetOutdoorLightsLatitude_ShouldReturnDefaultValue_WhenNotConfigured()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();

        // Act
        var latitude = config.GetOutdoorLightsLatitude();

        // Assert
        latitude.Should().Be(59.9139); // Oslo default
    }

    [Fact]
    public void GetOutdoorLightsLongitude_ShouldReturnConfiguredValue()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "-0.1278"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var longitude = config.GetOutdoorLightsLongitude();

        // Assert
        longitude.Should().Be(-0.1278);
    }

    [Fact]
    public void GetOutdoorLightsCheckInterval_ShouldReturnConfiguredValue()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES"] = "15"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var interval = config.GetOutdoorLightsCheckInterval();

        // Assert
        interval.Should().Be(TimeSpan.FromMinutes(15));
    }

    [Fact]
    public void GetOutdoorLightsCheckInterval_ShouldReturnDefaultValue_WhenNotConfigured()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();

        // Act
        var interval = config.GetOutdoorLightsCheckInterval();

        // Assert
        interval.Should().Be(TimeSpan.FromMinutes(30)); // Default
    }

    [Fact]
    public void GetOutdoorLightsSunriseOffset_ShouldReturnConfiguredValue()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "30"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var offset = config.GetOutdoorLightsSunriseOffset();

        // Assert
        offset.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public void GetOutdoorLightsSunsetOffset_ShouldReturnConfiguredValue()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "-30"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var offset = config.GetOutdoorLightsSunsetOffset();

        // Assert
        offset.Should().Be(TimeSpan.FromMinutes(-30));
    }

    [Fact]
    public void GetOutdoorLightsOptions_ShouldReturnAllConfiguredValues()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            ["OUTDOOR_LIGHTS_LATITUDE"] = "40.7128",
            ["OUTDOOR_LIGHTS_LONGITUDE"] = "-74.0060",
            ["OUTDOOR_LIGHTS_CHECK_INTERVAL_MINUTES"] = "45",
            ["OUTDOOR_LIGHTS_SUNRISE_OFFSET_MINUTES"] = "15",
            ["OUTDOOR_LIGHTS_SUNSET_OFFSET_MINUTES"] = "-20"
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();

        // Act
        var options = config.GetOutdoorLightsOptions();

        // Assert
        options.Latitude.Should().Be(40.7128);
        options.Longitude.Should().Be(-74.0060);
        options.CheckInterval.Should().Be(TimeSpan.FromMinutes(45));
        options.SunriseOffset.Should().Be(TimeSpan.FromMinutes(15));
        options.SunsetOffset.Should().Be(TimeSpan.FromMinutes(-20));
    }

    [Fact]
    public void GetOutdoorLightsOptions_ShouldReturnDefaultValues_WhenNotConfigured()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();

        // Act
        var options = config.GetOutdoorLightsOptions();

        // Assert
        options.Latitude.Should().Be(59.9139); // Oslo default
        options.Longitude.Should().Be(10.7522); // Oslo default
        options.CheckInterval.Should().Be(TimeSpan.FromMinutes(30));
        options.SunriseOffset.Should().Be(TimeSpan.Zero);
        options.SunsetOffset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void OutdoorLightsOptions_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var options = new OutdoorLightsOptions();

        // Assert
        options.Latitude.Should().Be(59.9139); // Oslo
        options.Longitude.Should().Be(10.7522); // Oslo
        options.CheckInterval.Should().Be(TimeSpan.FromMinutes(30));
        options.SunriseOffset.Should().Be(TimeSpan.Zero);
        options.SunsetOffset.Should().Be(TimeSpan.Zero);
    }
}