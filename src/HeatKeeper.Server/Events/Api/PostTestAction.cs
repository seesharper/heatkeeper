using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Post("api/actions")]
public record PostTestActionCommand(int ActionId, IReadOnlyDictionary<string, string> ParameterMap) : PostCommand;

public class PostTestAction(ActionCatalog catalog, ICommandExecutor commandExecutor) : ICommandHandler<PostTestActionCommand>
{
    public async Task HandleAsync(PostTestActionCommand command, CancellationToken cancellationToken = default)
    {
        var actionDetails = catalog.GetActionDetails(command.ActionId);

        // Create the command instance from the parameter map
        var actionCommand = TriggerEngine.CreateCommandFromParameters(actionDetails, command.ParameterMap);

        // Execute the command
        await commandExecutor.ExecuteAsync((dynamic)actionCommand, cancellationToken);
    }
}
