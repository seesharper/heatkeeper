using System;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;

public class DashboardForecastTests : TestBase
{
    // Use tomorrow so all period boundaries are within the Yr API forecast window
    private static readonly DateTime TomorrowUtc = DateTime.UtcNow.Date.AddDays(1);

    [Theory]
    [InlineData(2, 0)]   // 02:00 UTC falls in the 00-06 block
    [InlineData(8, 6)]   // 08:00 UTC falls in the 06-12 block
    [InlineData(14, 12)] // 14:00 UTC falls in the 12-18 block
    [InlineData(20, 18)] // 20:00 UTC falls in the 18-00 block
    public async Task ShouldReturnFirstPeriodMatchingCurrentBlock(int nowHour, int expectedBlockStartHour)
    {
        // The test location has no timezone set, so the handler uses UTC, making assertions straightforward
        Factory.UseFakeTimeProvider(TomorrowUtc.AddHours(nowHour));
        var testLocation = await Factory.CreateTestLocation();
        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();

        var forecast = await queryExecutor.ExecuteAsync(new DashboardForecastQuery(testLocation.LocationId));

        var expectedFrom = new DateTimeOffset(TomorrowUtc.AddHours(expectedBlockStartHour));
        forecast.FirstPeriod.From.Should().Be(expectedFrom);
        forecast.FirstPeriod.To.Should().Be(expectedFrom.AddHours(6));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(8)]
    [InlineData(14)]
    [InlineData(20)]
    public async Task ShouldReturnFourConsecutiveSixHourPeriods(int nowHour)
    {
        Factory.UseFakeTimeProvider(TomorrowUtc.AddHours(nowHour));
        var testLocation = await Factory.CreateTestLocation();
        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();

        var forecast = await queryExecutor.ExecuteAsync(new DashboardForecastQuery(testLocation.LocationId));

        forecast.FirstPeriod.Should().NotBeNull();
        forecast.SecondPeriod.From.Should().Be(forecast.FirstPeriod.From.AddHours(6));
        forecast.ThirdPeriod.From.Should().Be(forecast.FirstPeriod.From.AddHours(12));
        forecast.FourthPeriod.From.Should().Be(forecast.FirstPeriod.From.AddHours(18));

        forecast.FirstPeriod.To.Should().Be(forecast.FirstPeriod.From.AddHours(6));
        forecast.SecondPeriod.To.Should().Be(forecast.SecondPeriod.From.AddHours(6));
        forecast.ThirdPeriod.To.Should().Be(forecast.ThirdPeriod.From.AddHours(6));
        forecast.FourthPeriod.To.Should().Be(forecast.FourthPeriod.From.AddHours(6));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(8)]
    [InlineData(14)]
    [InlineData(20)]
    public async Task ShouldReturnInstantWithTemperatureAndSymbolCode(int nowHour)
    {
        Factory.UseFakeTimeProvider(TomorrowUtc.AddHours(nowHour));
        var testLocation = await Factory.CreateTestLocation();
        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();

        var forecast = await queryExecutor.ExecuteAsync(new DashboardForecastQuery(testLocation.LocationId));

        forecast.Instant.Should().NotBeNull();
        forecast.Instant.SymbolCode.Should().NotBeNullOrEmpty();
        forecast.Instant.Temperature.Should().NotBeNull();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(8)]
    [InlineData(14)]
    [InlineData(20)]
    public async Task ShouldReturnSymbolCodeAndTemperatureForAllPeriods(int nowHour)
    {
        Factory.UseFakeTimeProvider(TomorrowUtc.AddHours(nowHour));
        var testLocation = await Factory.CreateTestLocation();
        var queryExecutor = testLocation.ServiceProvider.GetRequiredService<IQueryExecutor>();

        var forecast = await queryExecutor.ExecuteAsync(new DashboardForecastQuery(testLocation.LocationId));

        foreach (var period in new[] { forecast.FirstPeriod, forecast.SecondPeriod, forecast.ThirdPeriod, forecast.FourthPeriod })
        {
            period.Should().NotBeNull();
            period.SymbolCode.Should().NotBeNullOrEmpty();
            period.Temperature.Should().NotBeNull();
        }
    }
}
