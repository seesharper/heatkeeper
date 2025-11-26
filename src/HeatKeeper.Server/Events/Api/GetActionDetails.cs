namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Get("api/actions/{actionId}")]
public sealed record GetActionDetailsQuery(int ActionId) : IQuery<ActionDetails>;

public sealed class GetActionDetails(ActionCatalog actionCatalog) : IQueryHandler<GetActionDetailsQuery, ActionDetails>
{
    public Task<ActionDetails> HandleAsync(GetActionDetailsQuery query, CancellationToken cancellationToken = default)
    {
        // Actions are registered at composition root; just fetch by ID
        var actionDetails = actionCatalog.GetActionDetails(query.ActionId);
        return Task.FromResult(actionDetails);
    }
}
