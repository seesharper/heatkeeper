namespace HeatKeeper.Server.Events.Api;

/// <summary>
/// Simplified event information containing ID and display name.
/// </summary>
/// <param name="Id">The unique event identifier</param>
/// <param name="Name">The human-readable event name</param>
public sealed record EventInfo(int Id, string Name);

[RequireUserRole]
[Get("api/events")]
public record GetEventsQuery : IQuery<EventInfo[]>;

public class GetEvents(IEventCatalog eventCatalog) : IQueryHandler<GetEventsQuery, EventInfo[]>
{
    public Task<EventInfo[]> HandleAsync(GetEventsQuery query, CancellationToken cancellationToken = default)
    {
        // Scan the server assembly to discover all available event types
        eventCatalog.ScanAssembly(typeof(EventBus).Assembly);

        // Return event ID and name from the enhanced metadata
        return Task.FromResult(eventCatalog.ListEventTypes()
            .Select(eventDef => new EventInfo(eventDef.Id, eventDef.Name))
            .ToArray());
    }
}