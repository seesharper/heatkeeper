using System;
using Cronos;
using Janitor;

namespace HeatKeeper.Server.Programs;

public class CronSchedule : ISchedule
{
    private readonly CronExpression _cronExpression;

    public CronSchedule(string cronScheduleExpression)
    {
        _cronExpression = CronExpression.Parse(cronScheduleExpression);
        CronScheduleExpression = cronScheduleExpression;
    }

    public string CronScheduleExpression { get; }

    public DateTime? GetNext(DateTime utcNow)
        => _cronExpression.GetNextOccurrence(utcNow);
}