namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Get("api/events")]
public record GetEventsQuery : IQuery<EventTypeInfo[]>;

public class GetEvents(IEventCatalog eventCatalog) : IQueryHandler<GetEventsQuery, EventTypeInfo[]>
{
    public Task<EventTypeInfo[]> HandleAsync(GetEventsQuery query, CancellationToken cancellationToken = default)
    {
        // Scan the server assembly to discover all available event types
        eventCatalog.ScanAssembly(typeof(EventBus).Assembly);

        // Return the discovered event types as an array
        return Task.FromResult(eventCatalog.ListEventTypes().ToArray());
    }
}