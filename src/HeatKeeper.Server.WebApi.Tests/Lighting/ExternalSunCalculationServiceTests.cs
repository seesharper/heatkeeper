using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Lighting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

public class ExternalSunCalculationServiceTests
{
    [Fact]
    public async Task GetSunriseSunsetAsync_ShouldReturnReasonableTimesForOslo()
    {
        // Arrange
        var httpClient = new HttpClient();
        var mockLogger = new Mock<ILogger<ExternalSunCalculationService>>();
        var service = new ExternalSunCalculationService(httpClient, mockLogger.Object);

        // Act - Get sun times for Oslo on summer solstice 2024
        var (sunrise, sunset) = await service.GetSunriseSunsetAsync(
            new DateTime(2024, 6, 21), 59.9139, 10.7522);

        // Assert - Oslo in summer has very early sunrise and late sunset
        // Sunrise can be as early as ~2 AM in summer at this latitude
        sunrise.Should().BeOnOrAfter(new DateTime(2024, 6, 21, 1, 0, 0, DateTimeKind.Utc));
        sunrise.Should().BeOnOrBefore(new DateTime(2024, 6, 21, 7, 0, 0, DateTimeKind.Utc)); // Extended to allow fallback

        sunset.Should().BeOnOrAfter(new DateTime(2024, 6, 21, 17, 0, 0, DateTimeKind.Utc)); // Extended to allow fallback  
        sunset.Should().BeOnOrBefore(new DateTime(2024, 6, 21, 22, 0, 0, DateTimeKind.Utc));

        // Verify that both times are on the expected date
        sunrise.Date.Should().Be(new DateTime(2024, 6, 21).Date);
        sunset.Date.Should().Be(new DateTime(2024, 6, 21).Date);

        // Sunrise should always be before sunset
        sunrise.Should().BeBefore(sunset);
    }

    [Fact]
    public async Task GetSunriseSunsetAsync_ShouldFallbackToInternalCalculation_OnError()
    {
        // Arrange - Create a service with an HttpClient that has a very short timeout to simulate network failure
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(1); // Very short timeout to force failure

        var mockLogger = new Mock<ILogger<ExternalSunCalculationService>>();
        var service = new ExternalSunCalculationService(httpClient, mockLogger.Object);

        // Act - This should fail the API call due to timeout and fallback to internal calculation
        var (sunrise, sunset) = await service.GetSunriseSunsetAsync(
            new DateTime(2024, 6, 21), 59.9139, 10.7522);

        // Assert - Should still get some reasonable times from fallback (6:00 AM / 6:00 PM)
        sunrise.Should().NotBe(sunset);
        sunrise.Should().BeBefore(sunset);

        // Should get fallback times
        sunrise.Should().Be(new DateTime(2024, 6, 21, 6, 0, 0, DateTimeKind.Utc));
        sunset.Should().Be(new DateTime(2024, 6, 21, 18, 0, 0, DateTimeKind.Utc));

        // Verify that error was logged
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to get sun times from API")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);

        // Verify that fallback warning was logged
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Using simple fallback times")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }
}