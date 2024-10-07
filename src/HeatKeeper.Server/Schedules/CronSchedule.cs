using System;
using Cronos;
using Janitor;

namespace HeatKeeper.Server.Schedules;


public class CronSchedule(string cronScheduleExpression) : ISchedule
{
    private readonly CronExpression _cronExpression = CronExpression.Parse(cronScheduleExpression);

    public string CronScheduleExpression { get; } = cronScheduleExpression;

    public DateTime? GetNext(DateTime utcNow)
        => _cronExpression.GetNextOccurrence(utcNow);
}