using Janitor;

namespace HeatKeeper.Server.Jobs;

[Post("api/jobs")]
[RequireUserRole]
public record PostRunJobCommand(string JobName);

public class PostRunJob(IJanitor janitor) : ICommandHandler<PostRunJobCommand>
{
    public async Task HandleAsync(PostRunJobCommand command, CancellationToken cancellationToken = default)
        => await janitor.Run(command.JobName);
}