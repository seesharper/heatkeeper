namespace HeatKeeper.Server.Events.Api;

/// <summary>
/// Simplified action information containing ID and display name.
/// </summary>
/// <param name="Id">The unique action identifier</param>
/// <param name="DisplayName">The human-readable display name</param>
public sealed record ActionInfo(int Id, string DisplayName);

[RequireUserRole]
[Get("api/actions")]
public record GetActionsQuery : IQuery<ActionInfo[]>;

public class GetActions(ActionCatalog actionCatalog) : IQueryHandler<GetActionsQuery, ActionInfo[]>
{
    public Task<ActionInfo[]> HandleAsync(GetActionsQuery query, CancellationToken cancellationToken = default)
    {
        // Actions are registered at composition root, just list and map
        var actions = actionCatalog.List()
            .Select(a => new ActionInfo(a.Id, a.DisplayName))
            .OrderBy(a => a.DisplayName)
            .ToArray();

        return Task.FromResult(actions);
    }
}
