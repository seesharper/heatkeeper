namespace HeatKeeper.Server.Events;

/// <summary>
/// Action binding says: when this trigger matches, invoke an action identified by ActionId with parameters from a map.
/// Values in ParameterMap can contain placeholders in the format {{propertyName}} which will be resolved against 
/// the domain event payload. For example, {{ZoneId}} will be replaced with the ZoneId property value from the event.
/// String values without placeholders are treated as literals.
/// </summary>
/// <param name="ActionId">The unique ID of the action to invoke</param>
/// <param name="ParameterMap">Parameter name to value mapping. Values can contain {{propertyName}} placeholders for dynamic resolution</param>
/// <example>
/// <code>
/// // Example: Disable heaters in the zone where a dead sensor was detected
/// new ActionBinding(
///     ActionId: 5,
///     ParameterMap: new Dictionary&lt;string, string&gt;
///     {
///         ["ZoneId"] = "{{ZoneId}}", // Will be resolved from event payload
///         ["Reason"] = "Dead sensor detected" // Literal value
///     }
/// )
/// </code>
/// </example>
public sealed record ActionBinding(int ActionId, IReadOnlyDictionary<string, string> ParameterMap);