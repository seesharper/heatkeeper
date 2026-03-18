using HeatKeeper.Server.EnergyCosts.Api;

namespace HeatKeeper.Server.EnergyCosts;

public static class TimePeriodCalculator
{
    public static (DateTime from, DateTime to) GetDateRange(TimePeriod timePeriod, DateTime now)
        => timePeriod switch
        {
            TimePeriod.Today => (now.Date, now),
            TimePeriod.Yesterday => (now.Date.AddDays(-1), now.Date),
            TimePeriod.LastWeek => (now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7) - 7), now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7))),
            TimePeriod.ThisWeek => (now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7)), now),
            TimePeriod.ThisMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now),
            TimePeriod.LastMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1), new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)),
            TimePeriod.ThisYear => (new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), now),
            TimePeriod.LastYear => (new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            _ => throw new ArgumentOutOfRangeException(nameof(timePeriod))
        };

    public static Resolution GetResolution(TimePeriod timePeriod)
        => timePeriod switch
        {
            TimePeriod.Today => Resolution.Hourly,
            TimePeriod.Yesterday => Resolution.Hourly,
            TimePeriod.LastWeek => Resolution.Daily,
            TimePeriod.ThisWeek => Resolution.Daily,
            TimePeriod.ThisMonth => Resolution.Daily,
            TimePeriod.LastMonth => Resolution.Daily,
            TimePeriod.ThisYear => Resolution.Monthly,
            TimePeriod.LastYear => Resolution.Monthly,
            _ => throw new ArgumentOutOfRangeException(nameof(timePeriod))
        };
}
