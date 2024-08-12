using HeatKeeper.Server.Schedules.Api;
using Janitor;

namespace HeatKeeper.Server.Schedules;


[RequireBackgroundRole]
public record AddScheduleToJanitorCommand(long ScheduleId, string Name, string CronExpression);

public class AddScheduleToJanitor(IJanitor janitor) : ICommandHandler<AddScheduleToJanitorCommand>
{
    private const string Prefix = "Schedule";

    public Task HandleAsync(AddScheduleToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        janitor.Schedule(builder =>
        {
            builder
                .WithName($"{Prefix}_{command.ScheduleId}")
                .WithSchedule(new CronSchedule(command.CronExpression))
                .WithStateHandler(async (TaskState state) => { })
                .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken ct) =>
                {
                    await commandExecutor.ExecuteAsync(new SetActiveScheduleCommand(command.ScheduleId), ct);
                });
        });

        return Task.CompletedTask;
    }
}
