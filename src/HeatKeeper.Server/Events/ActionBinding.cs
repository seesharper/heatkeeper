namespace HeatKeeper.Server.Events;

/// <summary>
/// Action binding says: when this trigger matches, invoke ActionName with parameters resolved from a map.
/// Values in ParameterMap can be literals or templates like "{{payload.ZoneId}}" or "{{trigger.Threshold}}".
/// </summary>
/// <param name="ActionName">The name of the action to invoke</param>
/// <param name="ParameterMap">Parameter name to value/template mapping</param>
public sealed record ActionBinding(string ActionName, IReadOnlyDictionary<string, string> ParameterMap);