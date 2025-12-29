using HeatKeeper.Server.Events;

namespace HeatKeeper.Server.Lighting;

/// <summary>
/// Strongly-typed payload for sunrise events.
/// </summary>
[DomainEvent(5, "Sunset Event", "Event triggered at sunset")]
public sealed record SunsetEvent([property: Lookup("api/locations")] long LocationId);
