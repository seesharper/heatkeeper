using System;
using System.Globalization;
using HeatKeeper.Server.EnergyCosts;
using HeatKeeper.Server.EnergyCosts.Api;

namespace HeatKeeper.Server.WebApi.Tests;

public class TimePeriodCalculatorTests
{
    [Theory]
    [InlineData("2026-03-16")] // Monday
    [InlineData("2026-03-17")] // Tuesday
    [InlineData("2026-03-18")] // Wednesday
    [InlineData("2026-03-19")] // Thursday
    [InlineData("2026-03-20")] // Friday
    [InlineData("2026-03-21")] // Saturday
    [InlineData("2026-03-22")] // Sunday
    public void LastWeek_AlwaysReturnsMonThroughSunOfPreviousCalendarWeek(string todayString)
    {
        var today = DateTime.Parse(todayString, null, DateTimeStyles.RoundtripKind);
        var (from, to) = TimePeriodCalculator.GetDateRange(TimePeriod.LastWeek, today);

        from.DayOfWeek.Should().Be(DayOfWeek.Monday);
        to.DayOfWeek.Should().Be(DayOfWeek.Monday);
        (to - from).TotalDays.Should().Be(7);
        to.Should().Be(today.Date.AddDays(-(((int)today.DayOfWeek + 6) % 7)));
    }

    [Fact]
    public void LastWeek_ReturnsCorrectDatesForKnownWeek()
    {
        // Wednesday March 18 → last week is March 9 (Mon) to March 16 (Mon exclusive = Sun inclusive)
        var now = new DateTime(2026, 3, 18, 14, 0, 0, DateTimeKind.Utc);
        var (from, to) = TimePeriodCalculator.GetDateRange(TimePeriod.LastWeek, now);

        from.Should().Be(new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc));
        to.Should().Be(new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc));
    }
}
