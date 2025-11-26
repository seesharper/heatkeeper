namespace HeatKeeper.Server.Events.Api;

[RequireUserRole]
[Get("api/events/{eventId}")]
public record GetEventDetailsQuery(int EventId) : IQuery<EventDetails>;

public class GetEventDetails(IEventCatalog eventCatalog) : IQueryHandler<GetEventDetailsQuery, EventDetails>
{
    public Task<EventDetails> HandleAsync(GetEventDetailsQuery query, CancellationToken cancellationToken = default)
    {
        // Scan the server assembly to discover all available event types
        eventCatalog.ScanAssembly(typeof(EventBus).Assembly);

        // Get the specific event details by ID
        var eventDetails = eventCatalog.GetEventDetails(query.EventId);

        return Task.FromResult(eventDetails);
    }
}