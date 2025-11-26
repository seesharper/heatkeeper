namespace HeatKeeper.Server.Events;

/// <summary>
/// Action binding says: when this trigger matches, invoke an action identified by ActionId with parameters from a map.
/// Values in ParameterMap are treated as literals and are not resolved against the domain event payload.
/// </summary>
/// <param name="ActionId">The unique ID of the action to invoke</param>
/// <param name="ParameterMap">Parameter name to literal value mapping</param>
public sealed record ActionBinding(int ActionId, IReadOnlyDictionary<string, string> ParameterMap);