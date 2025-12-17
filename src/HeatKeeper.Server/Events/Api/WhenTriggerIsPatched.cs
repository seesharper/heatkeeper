using HeatKeeper.Server.Events.Api;

namespace HeatKeeper.Server.Events;

public class WhenTriggerIsPatched(ICommandExecutor commandExecutor, ICommandHandler<PatchTriggerCommand> handler) : ICommandHandler<PatchTriggerCommand>
{
    public async Task HandleAsync(PatchTriggerCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        await commandExecutor.ExecuteAsync(new AddAllTriggersToTriggerEngineCommand(), cancellationToken);
    }
}
