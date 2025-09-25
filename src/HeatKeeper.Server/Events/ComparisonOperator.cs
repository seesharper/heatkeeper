namespace HeatKeeper.Server.Events;

/// <summary>
/// Operators for comparing values in trigger conditions.
/// Kept intentionally small so you can easily render them in a UI picker.
/// </summary>
public enum ComparisonOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterOrEqual,
    LessThan,
    LessOrEqual,
    Contains,       // string contains
    StartsWith,     // string starts with
    EndsWith        // string ends with
}