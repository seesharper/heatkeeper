using System;
using System.Text.Json;
using HeatKeeper.Server.Events;

namespace HeatKeeper.Server.WebApi.Tests;

/// <summary>
/// Unit tests for ConditionGroup evaluation logic in TriggerEngine.
/// These tests use a minimal in-process setup with no HTTP or database.
/// </summary>
public class TriggerEngineConditionTests
{
    // -----------------------------------------------------------------------
    // Helper: build a fake EventEnvelope with an anonymous payload
    // -----------------------------------------------------------------------

    private static EventEnvelope MakeEvent(object payload, int eventId = 1)
        => new(payload, eventId, payload.GetType().Name, DateTimeOffset.UtcNow);

    // -----------------------------------------------------------------------
    // Helper: run Matches via TriggerEngine.ConsumeAllEvents
    // We expose Evaluate indirectly through the engine itself.
    // -----------------------------------------------------------------------

    private static bool Evaluate(ConditionGroup? condition, object payload)
    {
        // Build a minimal trigger that executes no actions
        var trigger = new TriggerDefinition("test", 1, condition, []);
        var evt = MakeEvent(payload);

        // Use reflection to call the private static Evaluate method
        var method = typeof(TriggerEngine).GetMethod(
            "Evaluate",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        return (bool)method!.Invoke(null, [condition, evt])!;
    }

    // -----------------------------------------------------------------------
    // Null condition
    // -----------------------------------------------------------------------

    [Fact]
    public void NullCondition_AlwaysFires()
        => Evaluate(null, new { X = 1 }).Should().BeTrue();

    // -----------------------------------------------------------------------
    // Single Condition — comparison operators
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(25.1, true)]
    [InlineData(25.0, false)]
    [InlineData(10.0, false)]
    public void Condition_GreaterThan(double value, bool expected)
        => Evaluate(new Condition("Value", ComparisonOperator.GreaterThan, "25.0"), new { Value = value })
            .Should().Be(expected);

    [Theory]
    [InlineData(25.0, true)]
    [InlineData(25.1, true)]
    [InlineData(24.9, false)]
    public void Condition_GreaterOrEqual(double value, bool expected)
        => Evaluate(new Condition("Value", ComparisonOperator.GreaterOrEqual, "25.0"), new { Value = value })
            .Should().Be(expected);

    [Theory]
    [InlineData(24.9, true)]
    [InlineData(25.0, false)]
    [InlineData(25.1, false)]
    public void Condition_LessThan(double value, bool expected)
        => Evaluate(new Condition("Value", ComparisonOperator.LessThan, "25.0"), new { Value = value })
            .Should().Be(expected);

    [Theory]
    [InlineData(25.0, true)]
    [InlineData(24.9, true)]
    [InlineData(25.1, false)]
    public void Condition_LessOrEqual(double value, bool expected)
        => Evaluate(new Condition("Value", ComparisonOperator.LessOrEqual, "25.0"), new { Value = value })
            .Should().Be(expected);

    [Theory]
    [InlineData("hello", true)]
    [InlineData("world", false)]
    public void Condition_Equals_String(string value, bool expected)
        => Evaluate(new Condition("Name", ComparisonOperator.Equals, "hello"), new { Name = value })
            .Should().Be(expected);

    [Theory]
    [InlineData("hello", false)]
    [InlineData("world", true)]
    public void Condition_NotEquals(string value, bool expected)
        => Evaluate(new Condition("Name", ComparisonOperator.NotEquals, "hello"), new { Name = value })
            .Should().Be(expected);

    [Theory]
    [InlineData("hello world", true)]
    [InlineData("foo bar", false)]
    public void Condition_Contains(string value, bool expected)
        => Evaluate(new Condition("Name", ComparisonOperator.Contains, "hello"), new { Name = value })
            .Should().Be(expected);

    [Theory]
    [InlineData("hello world", true)]
    [InlineData("world hello", false)]
    public void Condition_StartsWith(string value, bool expected)
        => Evaluate(new Condition("Name", ComparisonOperator.StartsWith, "hello"), new { Name = value })
            .Should().Be(expected);

    [Theory]
    [InlineData("say hello", true)]
    [InlineData("hello world", false)]
    public void Condition_EndsWith(string value, bool expected)
        => Evaluate(new Condition("Name", ComparisonOperator.EndsWith, "hello"), new { Name = value })
            .Should().Be(expected);

    // -----------------------------------------------------------------------
    // LogicalCondition — AND
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(true,  true,  true)]   // both true → fires
    [InlineData(true,  false, false)]  // right false → does not fire
    [InlineData(false, true,  false)]  // left false → does not fire
    [InlineData(false, false, false)]  // both false → does not fire
    public void LogicalAnd(bool aMatch, bool bMatch, bool expected)
    {
        // A: Name == "Joe" (matches when aMatch), B: Value > 10 (matches when bMatch)
        var condition = new LogicalCondition(
            new Condition("Name",  ComparisonOperator.Equals,      "Joe"),
            LogicalOperator.And,
            new Condition("Value", ComparisonOperator.GreaterThan, "10"));

        var payload = new { Name = aMatch ? "Joe" : "NotJoe", Value = bMatch ? 20.0 : 5.0 };

        Evaluate(condition, payload).Should().Be(expected);
    }

    // -----------------------------------------------------------------------
    // LogicalCondition — OR
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(true,  true,  true)]   // both true → fires
    [InlineData(true,  false, true)]   // left true → fires
    [InlineData(false, true,  true)]   // right true → fires
    [InlineData(false, false, false)]  // both false → does not fire
    public void LogicalOr(bool aMatch, bool bMatch, bool expected)
    {
        var condition = new LogicalCondition(
            new Condition("Name",  ComparisonOperator.Equals,      "Joe"),
            LogicalOperator.Or,
            new Condition("Value", ComparisonOperator.GreaterThan, "10"));

        var payload = new { Name = aMatch ? "Joe" : "NotJoe", Value = bMatch ? 20.0 : 5.0 };

        Evaluate(condition, payload).Should().Be(expected);
    }

    // -----------------------------------------------------------------------
    // Nested: A && (B || C)
    // -----------------------------------------------------------------------

    [Theory]
    //                            A      B      C      expected
    [InlineData(true,  true,  true,  true)]
    [InlineData(true,  true,  false, true)]   // A && (B || false) → true
    [InlineData(true,  false, true,  true)]   // A && (false || C) → true
    [InlineData(true,  false, false, false)]  // A && (false || false) → false
    [InlineData(false, true,  true,  false)]  // false && _ → false
    [InlineData(false, true,  false, false)]
    [InlineData(false, false, true,  false)]
    [InlineData(false, false, false, false)]
    public void Nested_A_And_BOrC(bool a, bool b, bool c, bool expected)
    {
        // A: X > 0, B: Y == "yes", C: Z == "yes"
        var condition = new LogicalCondition(
            new Condition("X", ComparisonOperator.GreaterThan, "0"),
            LogicalOperator.And,
            new LogicalCondition(
                new Condition("Y", ComparisonOperator.Equals, "yes"),
                LogicalOperator.Or,
                new Condition("Z", ComparisonOperator.Equals, "yes")));

        var payload = new { X = a ? 1.0 : -1.0, Y = b ? "yes" : "no", Z = c ? "yes" : "no" };

        Evaluate(condition, payload).Should().Be(expected);
    }

    // -----------------------------------------------------------------------
    // Nested: (A || B) && C
    // -----------------------------------------------------------------------

    [Theory]
    //                            A      B      C      expected
    [InlineData(true,  true,  true,  true)]
    [InlineData(true,  false, true,  true)]   // (A || false) && C → true
    [InlineData(false, true,  true,  true)]   // (false || B) && C → true
    [InlineData(false, false, true,  false)]  // (false || false) && C → false
    [InlineData(true,  true,  false, false)]  // _ && false → false
    [InlineData(true,  false, false, false)]
    [InlineData(false, true,  false, false)]
    [InlineData(false, false, false, false)]
    public void Nested_AOrB_And_C(bool a, bool b, bool c, bool expected)
    {
        var condition = new LogicalCondition(
            new LogicalCondition(
                new Condition("X", ComparisonOperator.Equals, "yes"),
                LogicalOperator.Or,
                new Condition("Y", ComparisonOperator.Equals, "yes")),
            LogicalOperator.And,
            new Condition("Z", ComparisonOperator.GreaterThan, "0"));

        var payload = new { X = a ? "yes" : "no", Y = b ? "yes" : "no", Z = c ? 1.0 : -1.0 };

        Evaluate(condition, payload).Should().Be(expected);
    }

    // -----------------------------------------------------------------------
    // Deep nesting: A && (B || (C && D))
    // -----------------------------------------------------------------------

    [Theory]
    // A=T, B=T → always true (A && (T || _))
    [InlineData(true,  true,  true,  true,  true)]
    [InlineData(true,  true,  false, false, true)]
    // A=T, B=F → depends on (C && D)
    [InlineData(true,  false, true,  true,  true)]   // A && (F || (T && T)) → true
    [InlineData(true,  false, true,  false, false)]  // A && (F || (T && F)) → false
    [InlineData(true,  false, false, true,  false)]  // A && (F || (F && T)) → false
    // A=F → always false
    [InlineData(false, true,  true,  true,  false)]
    [InlineData(false, false, false, false, false)]
    public void DeepNested_A_And_BOrCD(bool a, bool b, bool c, bool d, bool expected)
    {
        var condition = new LogicalCondition(
            new Condition("A", ComparisonOperator.Equals, "yes"),
            LogicalOperator.And,
            new LogicalCondition(
                new Condition("B", ComparisonOperator.Equals, "yes"),
                LogicalOperator.Or,
                new LogicalCondition(
                    new Condition("C", ComparisonOperator.Equals, "yes"),
                    LogicalOperator.And,
                    new Condition("D", ComparisonOperator.Equals, "yes"))));

        var payload = new
        {
            A = a ? "yes" : "no",
            B = b ? "yes" : "no",
            C = c ? "yes" : "no",
            D = d ? "yes" : "no"
        };

        Evaluate(condition, payload).Should().Be(expected);
    }

    // -----------------------------------------------------------------------
    // JSON roundtrip
    // -----------------------------------------------------------------------

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void JsonRoundtrip_SingleCondition()
    {
        ConditionGroup original = new Condition("Temperature", ComparisonOperator.GreaterThan, "25.0");
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var restored = JsonSerializer.Deserialize<ConditionGroup>(json, JsonOptions);

        restored.Should().BeOfType<Condition>()
            .Which.Should().BeEquivalentTo((Condition)original);
    }

    [Fact]
    public void JsonRoundtrip_LogicalCondition_And()
    {
        ConditionGroup original = new LogicalCondition(
            new Condition("A", ComparisonOperator.Equals, "1"),
            LogicalOperator.And,
            new Condition("B", ComparisonOperator.Equals, "2"));

        var json = JsonSerializer.Serialize(original, JsonOptions);
        var restored = JsonSerializer.Deserialize<ConditionGroup>(json, JsonOptions);

        restored.Should().BeOfType<LogicalCondition>();
        var lc = (LogicalCondition)restored!;
        lc.Operator.Should().Be(LogicalOperator.And);
        lc.Left.Should().BeOfType<Condition>().Which.PropertyName.Should().Be("A");
        lc.Right.Should().BeOfType<Condition>().Which.PropertyName.Should().Be("B");
    }

    [Fact]
    public void JsonRoundtrip_NestedLogicalCondition()
    {
        ConditionGroup original = new LogicalCondition(
            new Condition("ZoneId", ComparisonOperator.Equals, "1"),
            LogicalOperator.And,
            new LogicalCondition(
                new Condition("Name", ComparisonOperator.Equals, "Joe"),
                LogicalOperator.Or,
                new Condition("Name", ComparisonOperator.Equals, "Ben")));

        var json = JsonSerializer.Serialize(original, JsonOptions);
        var restored = JsonSerializer.Deserialize<ConditionGroup>(json, JsonOptions);

        restored.Should().BeOfType<LogicalCondition>();
        var top = (LogicalCondition)restored!;
        top.Operator.Should().Be(LogicalOperator.And);
        top.Left.Should().BeOfType<Condition>().Which.PropertyName.Should().Be("ZoneId");
        top.Right.Should().BeOfType<LogicalCondition>().Which.Operator.Should().Be(LogicalOperator.Or);
    }

    [Fact]
    public void JsonRoundtrip_TriggerDefinition_NullCondition()
    {
        var original = new TriggerDefinition("test", 1, null, []);
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var restored = JsonSerializer.Deserialize<TriggerDefinition>(json, JsonOptions);

        restored!.Condition.Should().BeNull();
    }

    [Fact]
    public void JsonRoundtrip_TriggerDefinition_WithLogicalCondition()
    {
        var original = new TriggerDefinition(
            "test", 1,
            new LogicalCondition(
                new Condition("X", ComparisonOperator.GreaterThan, "10"),
                LogicalOperator.Or,
                new Condition("Y", ComparisonOperator.LessThan, "5")),
            []);

        var json = JsonSerializer.Serialize(original, JsonOptions);
        var restored = JsonSerializer.Deserialize<TriggerDefinition>(json, JsonOptions);

        restored!.Condition.Should().BeOfType<LogicalCondition>()
            .Which.Operator.Should().Be(LogicalOperator.Or);
    }
}
