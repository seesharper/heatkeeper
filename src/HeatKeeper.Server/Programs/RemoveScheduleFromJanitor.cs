using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;
using Janitor;

namespace HeatKeeper.Server.Programs;

[RequireAdminRole]
public record RemoveScheduleFromJanitorCommand(long ScheduleId);

public class RemoveScheduleFromJanitor : ICommandHandler<RemoveScheduleFromJanitorCommand>
{
    private readonly IJanitor _janitor;

    public RemoveScheduleFromJanitor(IJanitor janitor)
    {
        _janitor = janitor;
    }

    public async Task HandleAsync(RemoveScheduleFromJanitorCommand command, CancellationToken cancellationToken = default)
    {
        await _janitor.Delete($"Schedule_{command.ScheduleId}");
    }
}