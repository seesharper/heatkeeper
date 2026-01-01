#nullable enable

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// The main trigger engine that evaluates conditions and executes actions when triggers fire.
/// </summary>
/// <remarks>
/// Initializes a new trigger engine.
/// </remarks>
/// <param name="bus">The event bus to listen to</param>
/// <param name="catalog">The action catalog for resolving actions</param>
/// <param name="commandExecutor">The command executor for executing action commands</param>
public sealed class TriggerEngine(IEventBus bus, ActionCatalog catalog, ICommandExecutor commandExecutor)
{
    private ImmutableList<TriggerDefinition> _triggers = ImmutableList<TriggerDefinition>.Empty;

    /// <summary>
    /// Adds a trigger definition to the engine.
    /// </summary>
    /// <param name="trigger">The trigger to add</param>
    public void AddTrigger(TriggerDefinition trigger) =>
        ImmutableInterlocked.Update(ref _triggers, list => list.Add(trigger));

    /// <summary>
    /// Replaces all triggers with the provided collection.
    /// </summary>
    /// <param name="triggers">The new triggers to set</param>
    public void SetTriggers(IEnumerable<TriggerDefinition> triggers) =>
        ImmutableInterlocked.Update(ref _triggers, _ => [.. triggers]);

    /// <summary>
    /// Gets a read-only list of all registered triggers.
    /// </summary>
    /// <returns>A read-only list of trigger definitions</returns>
    public IReadOnlyList<TriggerDefinition> ListTriggers() => _triggers;

    /// <summary>
    /// Starts the trigger engine, listening for events and processing triggers.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task that runs until cancelled</returns>
    public async Task StartAsync(CancellationToken ct)
    {
        await foreach (var evt in bus.Reader.ReadAllAsync(ct))
        {
            await ProcessEvent(evt, ct);
        }
    }

    /// <summary>
    /// Consumes all pending events in the event bus and processes triggers for testing scenarios.
    /// This method will process all events currently in the queue and then return.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task that completes when all pending events have been processed</returns>
    public async Task ConsumeAllEvents(CancellationToken ct = default)
    {
        while (bus.Reader.TryRead(out var evt))
        {
            await ProcessEvent(evt, ct);
        }
    }

    private async Task ProcessEvent(EventEnvelope evt, CancellationToken ct)
    {
        foreach (var trig in _triggers)
        {
            if (evt.EventId != trig.EventId)
                continue;

            if (Matches(evt, trig))
            {
                foreach (var binding in trig.Actions)
                {
                    ActionDetails actionDetails;
                    try
                    {
                        actionDetails = catalog.GetActionDetails(binding.ActionId);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"[WARN] Unknown action ID '{binding.ActionId}' in trigger '{trig.Name}'.");
                        continue;
                    }

                    var resolved = ResolveParameters(binding.ParameterMap, evt);

                    try
                    {
                        // Create the command from the resolved parameters
                        var command = CreateCommandFromParameters(actionDetails, resolved);

                        // Execute the command using ICommandExecutor
                        await commandExecutor.ExecuteAsync((dynamic)command, ct);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to execute action '{actionDetails.Name}': {ex.Message}");
                        continue;
                    }
                }
            }
        }
    }

    private static bool Matches(EventEnvelope evt, TriggerDefinition trig)
    {
        foreach (var c in trig.Conditions)
        {
            var left = ReadValueFromPayload(evt.Payload, c.PropertyName);
            var right = c.Value;

            if (!Compare(left, right, c.Operator))
                return false;
        }
        return true;
    }

    private static object? ReadValueFromPayload(object payload, string key)
    {
        // Use reflection to get the property value from the payload
        var payloadType = payload.GetType();
        var property = payloadType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        return property?.GetValue(payload);
    }

    private static bool Compare(object? left, object? right, ComparisonOperator op)
    {
        // Try numeric compare first
        if (TryToDouble(left, out var dl) && TryToDouble(right, out var dr))
        {
            return op switch
            {
                ComparisonOperator.Equals => dl == dr,
                ComparisonOperator.NotEquals => dl != dr,
                ComparisonOperator.GreaterThan => dl > dr,
                ComparisonOperator.GreaterOrEqual => dl >= dr,
                ComparisonOperator.LessThan => dl < dr,
                ComparisonOperator.LessOrEqual => dl <= dr,
                _ => throw new InvalidOperationException($"Operator {op} not valid for numbers.")
            };
        }

        var ls = left is JsonElement jl ? jl.ToString() : left?.ToString() ?? string.Empty;
        var rs = right is JsonElement jr ? jr.ToString() : right?.ToString() ?? string.Empty;
        return op switch
        {
            ComparisonOperator.Equals => string.Equals(ls, rs, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.NotEquals => !string.Equals(ls, rs, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.Contains => ls.Contains(rs, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.StartsWith => ls.StartsWith(rs, StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.EndsWith => ls.EndsWith(rs, StringComparison.OrdinalIgnoreCase),
            _ => throw new InvalidOperationException($"Operator {op} not valid for strings.")
        };
    }

    private static bool TryToDouble(object? v, out double d)
    {
        switch (v)
        {
            case null: d = default; return false;
            case double dv: d = dv; return true;
            case float fv: d = fv; return true;
            case int iv: d = iv; return true;
            case long lv: d = lv; return true;
            case decimal dec: d = (double)dec; return true;
            case string s when double.TryParse(s, out var parsed): d = parsed; return true;
            case JsonElement je when je.ValueKind == JsonValueKind.Number && je.TryGetDouble(out var num): d = num; return true;
            case JsonElement je when je.ValueKind == JsonValueKind.String && double.TryParse(je.GetString(), out var parsed2): d = parsed2; return true;
            default: d = default; return false;
        }
    }

    private static IReadOnlyDictionary<string, object?> ResolveParameters(
        IReadOnlyDictionary<string, string> parameterMap,
        EventEnvelope evt)
    {
        // Resolve parameter values, replacing {{propertyName}} placeholders with actual values from the event payload
        var dict = new Dictionary<string, object?>();
        foreach (var kvp in parameterMap)
        {
            dict[kvp.Key] = PropertyResolver.ResolveValue(kvp.Value, evt.Payload);
        }

        return new ReadOnlyDictionary<string, object?>(dict);
    }

    /// <summary>
    /// Creates a command instance from an ActionDetails and parameter map.
    /// </summary>
    /// <param name="actionDetails">The action details containing the command type information</param>
    /// <param name="parameterMap">String parameter map</param>
    /// <returns>A command instance ready to be executed</returns>
    public static object CreateCommandFromParameters(ActionDetails actionDetails, IReadOnlyDictionary<string, string> parameterMap)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var kvp in parameterMap)
        {
            dict[kvp.Key] = kvp.Value;
        }
        return CreateCommandFromParameters(actionDetails, new ReadOnlyDictionary<string, object?>(dict));
    }

    /// <summary>
    /// Creates a command instance from an ActionDetails and parameter dictionary.
    /// </summary>
    /// <param name="actionDetails">The action details containing the command type information</param>
    /// <param name="parameters">Parameter dictionary</param>
    /// <returns>A command instance ready to be executed</returns>
    public static object CreateCommandFromParameters(ActionDetails actionDetails, IReadOnlyDictionary<string, object?> parameters)
    {
        // Find the command type by looking for a type with the [Action] attribute matching the action ID
        var assembly = typeof(TriggerEngine).Assembly;
        var commandType = assembly.GetTypes()
            .FirstOrDefault(t => t.GetCustomAttribute<ActionAttribute>()?.Id == actionDetails.Id);

        if (commandType == null)
        {
            throw new InvalidOperationException($"Could not find command type for action ID {actionDetails.Id}");
        }

        // Serialize to JSON and deserialize to the command type
        // This handles property name mapping and type conversion
        var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var result = JsonSerializer.Deserialize(json, commandType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        });

        if (result == null)
        {
            throw new InvalidOperationException($"Failed to deserialize parameters to {commandType.Name}");
        }

        return result;
    }
}