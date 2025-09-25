namespace HeatKeeper.Server.Events;

/// <summary>
/// Represents action information that can be deserialized from JSON.
/// </summary>
public sealed class ActionInfo
{
    /// <summary>
    /// The unique name of the action.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name for the action.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Schema describing the parameters this action accepts.
    /// </summary>
    public required IReadOnlyList<ActionParameter> ParameterSchema { get; init; }

    /// <summary>
    /// The parameters to pass to the action when executed.
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();
}