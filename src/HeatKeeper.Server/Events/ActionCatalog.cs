using System.Collections.Concurrent;

namespace HeatKeeper.Server.Events;

/// <summary>
/// A catalog for registering and retrieving action metadata.
/// </summary>
public sealed class ActionCatalog
{
    private readonly ConcurrentDictionary<string, ActionDetails> _actions = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers an action in the catalog.
    /// </summary>
    /// <param name="action">The action metadata to register</param>
    public void Register(ActionDetails action) => _actions[action.Name] = action;

    /// <summary>
    /// Attempts to retrieve action metadata by name.
    /// </summary>
    /// <param name="name">The action name</param>
    /// <param name="action">The retrieved action metadata, if found</param>
    /// <returns>True if the action was found, false otherwise</returns>
    public bool TryGet(string name, out ActionDetails action) => _actions.TryGetValue(name, out action!);

    /// <summary>
    /// Lists all registered actions with their metadata.
    /// </summary>
    /// <returns>A list of action metadata (name, display name, parameter schema)</returns>
    public IReadOnlyList<ActionDetails> List()
        => _actions.Values
            .OrderBy(a => a.DisplayName)
            .ToList();

    /// <summary>
    /// Gets action details by action ID.
    /// </summary>
    /// <param name="id">The action ID</param>
    /// <returns>Action details if found</returns>
    public ActionDetails GetActionDetails(int id)
        => _actions.Values.First(a => a.Id == id);
}