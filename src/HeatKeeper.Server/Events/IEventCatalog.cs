using System.Reflection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Interface for the event catalog that discovers and lists available event types.
/// </summary>
public interface IEventCatalog
{
    /// <summary>
    /// Scans the specified assembly for types that can be used as DomainEvent payloads.
    /// </summary>
    /// <param name="assembly">The assembly to scan. If null, scans the calling assembly.</param>
    void ScanAssembly(Assembly? assembly = null);

    /// <summary>
    /// Gets all discovered event types.
    /// </summary>
    /// <returns>A read-only list of event type information</returns>
    IReadOnlyList<EventDetails> ListEventTypes();

    /// <summary>
    /// Gets event details by event ID.
    /// </summary>
    /// <param name="id">The event ID</param>
    /// <returns>Event details if found</returns>
    EventDetails GetEventDetails(int id);

    /// <summary>
    /// Clears all discovered event types.
    /// </summary>
    void Clear();
}