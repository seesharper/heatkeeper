using System;
using Cronos;
using Janitor;

namespace HeatKeeper.Server.Host.BackgroundTasks;

public class CronSchedule : ISchedule
{
    private readonly CronExpression _cronExpression;

    public CronSchedule(string cronExpression)
        => _cronExpression = CronExpression.Parse(cronExpression);

    public DateTime? GetNext(DateTime utcNow)
        => _cronExpression.GetNextOccurrence(utcNow);
}