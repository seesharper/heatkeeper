using HeatKeeper.Server.Schedules;
using Janitor;

namespace HeatKeeper.Server.Lighting;




[RequireBackgroundRole]
public record ReScheduleSunriseAndSunsetEventsCommand();


public class ReScheduleSunriseAndSunsetEvents(ICommandExecutor commandExecutor, TimeProvider timeProvider, IJanitor janitor) : ICommandHandler<ReScheduleSunriseAndSunsetEventsCommand>
{
    public Task HandleAsync(ReScheduleSunriseAndSunsetEventsCommand command, CancellationToken cancellationToken = default)
    {
        // Every day at noon 
        var cronExpression = "0 12 * * *";
        var todayUtc = timeProvider.GetUtcNow().Date;
        var tomorrowUtc = DateOnly.FromDateTime(todayUtc.AddDays(1));
        janitor.Schedule(builder =>
        {
            builder
                .WithName("Reschedule_Sunrise_Sunset_Events")
                .WithSchedule(new CronSchedule(cronExpression))
                .WithScheduledTask(async (ICommandExecutor commandExecutor) =>
                {
                    await commandExecutor.ExecuteAsync(new ScheduleSunriseAndSunsetEventsCommand(tomorrowUtc), cancellationToken);
                })
                .Build();
        });

        return Task.CompletedTask;
    }
}