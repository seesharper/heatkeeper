using System;
using Cronos;
using Janitor;

namespace HeatKeeper.Server.Schedules;


public class CronSchedule(string cronScheduleExpression) : ISchedule
{
    private readonly CronExpression _cronExpression = CronExpression.Parse(
        cronScheduleExpression,
        cronScheduleExpression.Split(' ').Length == 6 ? CronFormat.IncludeSeconds : CronFormat.Standard);

    public string CronScheduleExpression { get; } = cronScheduleExpression;

    public DateTime? GetNext(DateTime utcNow)
        => _cronExpression.GetNextOccurrence(utcNow);
}