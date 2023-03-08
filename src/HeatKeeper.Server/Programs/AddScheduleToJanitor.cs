using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using Janitor;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record AddScheduleToJanitorCommand(long ScheduleId, long ProgramId, string Name, string CronExpression);

public class AddScheduleToJanitorCommandHandler : ICommandHandler<AddScheduleToJanitorCommand>
{
    private readonly IJanitor _janitor;
    private const string Prefix = "Schedule";

    public AddScheduleToJanitorCommandHandler(IJanitor janitor)
        => _janitor = janitor;

    public Task HandleAsync(AddScheduleToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        _janitor.Schedule(builder =>
        {
            builder
                .WithName($"{Prefix}_{command.Name}")
                .WithSchedule(new CronSchedule(command.CronExpression))
                .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken ct) =>
                {
                    await commandExecutor.ExecuteAsync(new SetActiveScheduleCommand(command.ScheduleId), ct);
                });
        });

        return Task.CompletedTask;
    }
}
