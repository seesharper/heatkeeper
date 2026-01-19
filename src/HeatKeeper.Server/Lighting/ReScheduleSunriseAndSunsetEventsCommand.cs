using HeatKeeper.Server.Schedules;
using Janitor;
using Org.BouncyCastle.Asn1.Cms;

namespace HeatKeeper.Server.Lighting;




[RequireBackgroundRole]
public record ReScheduleSunriseAndSunsetEventsCommand();


public class ReScheduleSunriseAndSunsetEvents(IJanitor janitor) : ICommandHandler<ReScheduleSunriseAndSunsetEventsCommand>
{
    public Task HandleAsync(ReScheduleSunriseAndSunsetEventsCommand command, CancellationToken cancellationToken = default)
    {
        // Every day at noon 
        var cronExpression = "0 12 * * *";

        janitor.Schedule(builder =>
        {
            builder
                .WithName("Reschedule_Sunrise_Sunset_Events")
                .WithSchedule(new CronSchedule(cronExpression))
                .WithScheduledTask(async (ICommandExecutor commandExecutor, TimeProvider timeProvider) =>
                {
                    var todayUtc = timeProvider.GetUtcNow().Date;
                    var tomorrowUtc = DateOnly.FromDateTime(todayUtc.AddDays(1));
                    await commandExecutor.ExecuteAsync(new ScheduleSunriseAndSunsetEventsCommand(tomorrowUtc), cancellationToken);
                });
        });

        return Task.CompletedTask;
    }
}