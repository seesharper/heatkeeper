namespace HeatKeeper.Server.Events;

/// <summary>
/// Attribute to mark classes as actions and provide metadata about them.
/// </summary>
/// <param name="Id">Unique identifier for the action type</param>
/// <param name="Name">Human-readable name for the action</param>
/// <param name="Description">Description of what this action does</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ActionAttribute(int Id, string Name, string Description) : Attribute
{
    /// <summary>
    /// Unique identifier for the action type.
    /// </summary>
    public int Id { get; } = Id;

    /// <summary>
    /// Human-readable name for the action.
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    /// Description of what this action does.
    /// </summary>
    public string Description { get; } = Description;
}
