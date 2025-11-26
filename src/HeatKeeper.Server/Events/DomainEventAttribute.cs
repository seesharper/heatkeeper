namespace HeatKeeper.Server.Events;

/// <summary>
/// Attribute to mark classes as domain events and provide metadata about them.
/// </summary>
/// <param name="Id">Unique identifier for the event type</param>
/// <param name="Name">Human-readable name for the event</param>
/// <param name="Description">Description of what this event represents</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DomainEventAttribute(int Id, string Name, string Description) : Attribute
{
    /// <summary>
    /// Unique identifier for the event type.
    /// </summary>
    public int Id { get; } = Id;

    /// <summary>
    /// Human-readable name for the event.
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    /// Description of what this event represents.
    /// </summary>
    public string Description { get; } = Description;
}