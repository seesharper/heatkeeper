using Newtonsoft.Json;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Defines a complete trigger with its conditions and actions.
/// </summary>
/// <param name="Name">Human-readable name for the trigger</param>
/// <param name="AppliesToEventType">The event type this trigger applies to (e.g. "TemperatureReading")</param>
/// <param name="Values">Trigger-defined values (e.g. Threshold, TargetState)</param>
/// <param name="Conditions">List of conditions that must all be true for the trigger to fire</param>
/// <param name="Actions">List of actions to execute when the trigger fires</param>
public sealed record TriggerDefinition(
    string Name,
    string AppliesToEventType,
    IReadOnlyDictionary<string, object> Values,
    IReadOnlyList<Condition> Conditions,
    IReadOnlyList<ActionBinding> Actions
);