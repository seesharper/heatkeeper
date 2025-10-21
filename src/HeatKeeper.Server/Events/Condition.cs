namespace HeatKeeper.Server.Events;

/// <summary>
/// A single condition that compares an event payload property to a literal value.
/// </summary>
/// <param name="PropertyName">The property name from the event payload (e.g. "Temperature" or "ZoneId")</param>
/// <param name="Operator">The comparison operator to use</param>
/// <param name="Value">The literal value to compare against (string/number)</param>
public sealed record Condition
(
    string PropertyName,
    ComparisonOperator Operator,
    string Value
);