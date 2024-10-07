
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public class SetNoContentResultOnDeleteCommands<TCommand>(ICommandHandler<TCommand> handler) : ICommandHandler<TCommand> where TCommand : DeleteCommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        command.SetResult(TypedResults.NoContent());
    }
}