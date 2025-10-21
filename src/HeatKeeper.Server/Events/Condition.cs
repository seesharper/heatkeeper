namespace HeatKeeper.Server.Events;

/// <summary>
/// A single condition that can read from the event payload or use literal values.
/// </summary>
/// <param name="LeftSource">"payload" to read from event payload</param>
/// <param name="LeftKey">e.g. "Temperature" or "ZoneId"</param>
/// <param name="Operator">The comparison operator to use</param>
/// <param name="RightSource">"literal" or "payload"</param>
/// <param name="RightKeyOrLiteral">key if payload, otherwise literal value (string/number)</param>
public sealed record Condition
(
    string LeftSource,
    string LeftKey,
    ComparisonOperator Operator,
    string RightSource,
    string RightKeyOrLiteral
);