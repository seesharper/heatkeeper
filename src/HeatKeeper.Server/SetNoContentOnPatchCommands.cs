using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server;


public class SetNoContentResultOnPatchCommands<TCommand>(ICommandHandler<TCommand> handler) : ICommandHandler<TCommand> where TCommand : PatchCommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        command.SetResult(TypedResults.NoContent());
    }
}