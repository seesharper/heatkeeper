namespace HeatKeeper.Server.Events;

/// <summary>
/// Represents action information that can be deserialized from JSON.
/// </summary>
public sealed class ActionDetails
{
    /// <summary>
    /// Unique identifier for the action type.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// The unique name of the action.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name for the action.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Description of what this action does.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Schema describing the parameters this action accepts.
    /// </summary>
    public required IReadOnlyList<ActionParameter> ParameterSchema { get; init; }
}