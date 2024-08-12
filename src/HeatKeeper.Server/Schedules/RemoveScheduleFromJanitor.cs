using Janitor;

namespace HeatKeeper.Server.Schedules;

[RequireAdminRole]
public record RemoveScheduleFromJanitorCommand(long ScheduleId);

public class RemoveScheduleFromJanitor(IJanitor janitor) : ICommandHandler<RemoveScheduleFromJanitorCommand>
{
    public async Task HandleAsync(RemoveScheduleFromJanitorCommand command, CancellationToken cancellationToken = default)
        => await janitor.Delete($"Schedule_{command.ScheduleId}");
}