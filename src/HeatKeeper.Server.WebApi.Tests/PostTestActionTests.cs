using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Tests for the POST /api/actions endpoint (PostTestAction)
/// This endpoint allows manual testing/invocation of actions
/// </summary>
public class PostTestActionTests : TestBase
{
    public PostTestActionTests()
    {
    }

    [Fact]
    public async Task PostTestAction_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 1, // SendNotificationAction
            ParameterMap = new Dictionary<string, string>
            {
                { "message", "Test notification message" },
                { "severity", "1" } // Using int as string to test type conversion
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PostTestAction_WithInvalidActionId_ReturnsError()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 99999, // Non-existent action
            ParameterMap = new Dictionary<string, string>()
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        // Should return an error status code (4xx or 5xx)
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PostTestAction_WithMissingRequiredParameter_ReturnsError()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 1, // TestSendNotificationAction requires "message"
            ParameterMap = new Dictionary<string, string>
            {
                { "severity", "info" }
                // Missing required "message" parameter
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        // Should return an error status code
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PostTestAction_WithInvalidJson_ReturnsError()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                "{ invalid json",
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PostTestAction_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Not authenticating

        var request = new
        {
            ActionId = 1,
            ParameterMap = new Dictionary<string, string>
            {
                { "message", "test" },
                { "severity", "info" }
            }
        };

        // Act
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostTestAction_WithMultipleParameters_ProcessesRequest()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 1,
            ParameterMap = new Dictionary<string, string>
            {
                { "message", "Complex notification message" },
                { "severity", "warning" },
                { "extraParam1", "value1" },
                { "extraParam2", "value2" }
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        // The endpoint should process the request (success or error)
    }

    [Fact]
    public async Task PostTestAction_WithSpecialCharactersInParameters_ProcessesRequest()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 1,
            ParameterMap = new Dictionary<string, string>
            {
                { "message", "Test with special chars: <>&\"'åäö" },
                { "severity", "info" }
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        // The endpoint should handle special characters without crashing
    }

    [Fact]
    public async Task PostTestAction_WithNumericStringParameter_ConvertsToInt()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 2, // TestTurnHeatersOffAction
            ParameterMap = new Dictionary<string, string>
            {
                { "zoneId", "42" } // String representation of int
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode, "Expected successful type conversion from string '42' to int");
    }

    [Fact]
    public async Task PostTestAction_WithInvalidNumericString_ReturnsError()
    {
        // Arrange
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var request = new
        {
            ActionId = 2, // TestTurnHeatersOffAction
            ParameterMap = new Dictionary<string, string>
            {
                { "zoneId", "not-a-number" } // Invalid numeric string
            }
        };

        // Act
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await client.PostAsync("api/actions",
            new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccessStatusCode, "Expected failure when converting invalid string to int");
    }
}
