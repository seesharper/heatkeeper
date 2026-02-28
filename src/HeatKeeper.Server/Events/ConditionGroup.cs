using System.Text.Json.Serialization;

namespace HeatKeeper.Server.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Condition),        "condition")]
[JsonDerivedType(typeof(LogicalCondition), "logical")]
public abstract record ConditionGroup;

public sealed record Condition(string PropertyName, ComparisonOperator Operator, string Value)
    : ConditionGroup;

public sealed record LogicalCondition(ConditionGroup Left, LogicalOperator Operator, ConditionGroup Right)
    : ConditionGroup;

public enum LogicalOperator { And, Or }
