using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Post("api/actions")]
public record PostTestActionCommand(int ActionId, IReadOnlyDictionary<string, string> ParameterMap) : PostCommand;

public class PostTestAction(ActionCatalog catalog, IServiceProvider serviceProvider) : ICommandHandler<PostTestActionCommand>
{
    public async Task HandleAsync(PostTestActionCommand command, CancellationToken cancellationToken = default)
    {
        var actionDetails = catalog.GetActionDetails(command.ActionId);
        
        using var scope = serviceProvider.CreateScope();
        var serviceFactory = scope.ServiceProvider as LightInject.IServiceFactory
            ?? throw new InvalidOperationException("Expected LightInject service factory");

        var action = (IAction)serviceFactory.GetInstance(typeof(IAction), actionDetails.Name);
        
        await TriggerEngine.InvokeActionAsync(action, command.ParameterMap, cancellationToken);
    }
}
