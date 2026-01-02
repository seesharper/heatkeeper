using HeatKeeper.Server.Events;

namespace HeatKeeper.Server.Lighting;


/// <summary>
/// Strongly-typed payload for sundown events.
/// </summary>
[DomainEvent(4, "Sunrise Event", "Event triggered at sunrise")]
public sealed record SunriseEvent([property: Lookup("api/locations")] long LocationId);
