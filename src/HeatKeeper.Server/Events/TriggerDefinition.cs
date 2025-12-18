using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Defines a complete trigger with its conditions and actions.
/// </summary>
/// <param name="Name">Human-readable name for the trigger</param>
/// <param name="EventId">The event ID this trigger applies to (from DomainEvent attribute)</param>
/// <param name="Conditions">List of conditions that must all be true for the trigger to fire</param>
/// <param name="Actions">List of actions to execute when the trigger fires</param>
public sealed record TriggerDefinition(

    [property: Key] string Name,
    int EventId,
    IReadOnlyList<Condition> Conditions,
    IReadOnlyList<ActionBinding> Actions
);