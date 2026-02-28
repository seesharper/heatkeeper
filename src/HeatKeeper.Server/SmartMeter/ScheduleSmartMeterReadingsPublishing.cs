using HeatKeeper.Server.Schedules;
using Janitor;

namespace HeatKeeper.Server.SmartMeter;

[RequireBackgroundRole]
public record ScheduleSmartMeterReadingsPublishingCommand;

public class ScheduleSmartMeterReadingsPublishing(IJanitor janitor) : ICommandHandler<ScheduleSmartMeterReadingsPublishingCommand>
{
    public Task HandleAsync(ScheduleSmartMeterReadingsPublishingCommand command, CancellationToken cancellationToken = default)
    {
        janitor.Schedule(builder =>
        {
            builder
                .WithName("SmartMeterReadingsPublisher")
                .WithSchedule(new CronSchedule("*/10 * * * * *"))
                .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken ct) =>
                {
                    await commandExecutor.ExecuteAsync(new PublishSmartMeterReadingsCommand(), ct);
                });
        });

        return Task.CompletedTask;
    }
}
