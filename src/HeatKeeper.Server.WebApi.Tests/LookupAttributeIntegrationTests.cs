using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HeatKeeper.Server.Events;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Integration tests for the Lookup attribute feature through the API
/// </summary>
public class LookupAttributeIntegrationTests : TestBase
{
    [Fact]
    public async Task GetEventDetails_ForMotionDetected_ReturnsZoneIdWithLookupUrl()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var response = await client.GetAsync($"api/events/2?access_token={token}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var eventDetails = JsonSerializer.Deserialize<EventDetails>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(eventDetails);
        var zoneIdProperty = eventDetails.Properties.FirstOrDefault(p => p.Name == "ZoneId");
        Assert.NotNull(zoneIdProperty);
        Assert.Equal("api/locations/{locationId}/zones", zoneIdProperty.LookupUrl);
    }

    [Fact]
    public async Task GetActionDetails_ForTestTurnHeatersOff_ReturnsZoneIdWithLookupUrl()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act - TestTurnHeatersOffCommand has action ID -2
        var response = await client.GetAsync($"api/actions/-2?access_token={token}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var actionDetails = JsonSerializer.Deserialize<ActionDetails>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(actionDetails);
        var zoneIdParameter = actionDetails.ParameterSchema.FirstOrDefault(p => p.Name == "ZoneId");
        Assert.NotNull(zoneIdParameter);
        Assert.Equal("api/zones", zoneIdParameter.LookupUrl);
    }

    [Fact]
    public async Task GetEventDetails_PropertyWithoutLookup_ReturnsNullLookupUrl()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        var response = await client.GetAsync($"api/events/1?access_token={token}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var eventDetails = JsonSerializer.Deserialize<EventDetails>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(eventDetails);
        var temperatureProperty = eventDetails.Properties.FirstOrDefault(p => p.Name == "Temperature");
        Assert.NotNull(temperatureProperty);
        Assert.Null(temperatureProperty.LookupUrl);
    }
}
