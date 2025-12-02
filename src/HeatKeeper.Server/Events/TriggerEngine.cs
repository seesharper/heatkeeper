using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// The main trigger engine that evaluates conditions and executes actions when triggers fire.
/// </summary>
public sealed class TriggerEngine
{
    private readonly IEventBus _bus;
    private readonly ActionCatalog _catalog;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<TriggerDefinition> _triggers = new();

    /// <summary>
    /// Initializes a new trigger engine.
    /// </summary>
    /// <param name="bus">The event bus to listen to</param>
    /// <param name="catalog">The action catalog for resolving actions</param>
    /// <param name="serviceProvider">The service provider for creating action instances</param>
    public TriggerEngine(IEventBus bus, ActionCatalog catalog, IServiceProvider serviceProvider)
    {
        _bus = bus;
        _catalog = catalog;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Adds a trigger definition to the engine.
    /// </summary>
    /// <param name="trigger">The trigger to add</param>
    public void AddTrigger(TriggerDefinition trigger) => _triggers.Add(trigger);

    /// <summary>
    /// Replaces all triggers with the provided collection.
    /// </summary>
    /// <param name="triggers">The new triggers to set</param>
    public void SetTriggers(IEnumerable<TriggerDefinition> triggers)
    {
        _triggers.Clear();
        _triggers.AddRange(triggers);
    }

    /// <summary>
    /// Gets a read-only list of all registered triggers.
    /// </summary>
    /// <returns>A read-only list of trigger definitions</returns>
    public IReadOnlyList<TriggerDefinition> ListTriggers() => _triggers.AsReadOnly();

    /// <summary>
    /// Starts the trigger engine, listening for events and processing triggers.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task that runs until cancelled</returns>
    public async Task StartAsync(CancellationToken ct)
    {
        await foreach (var evt in _bus.Reader.ReadAllAsync(ct))
        {
            foreach (var trig in _triggers)
            {
                if (!evt.EventType.Equals(trig.AppliesToEventType, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (Matches(evt, trig))
                {
                    foreach (var binding in trig.Actions)
                    {
                        ActionDetails actionDetails;
                        try
                        {
                            actionDetails = _catalog.GetActionDetails(binding.ActionId);
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine($"[WARN] Unknown action ID '{binding.ActionId}' in trigger '{trig.Name}'.");
                            continue;
                        }

                        var resolved = ResolveParameters(binding.ParameterMap, evt);

                        // Create a new scope and resolve the action from DI container
                        using var scope = _serviceProvider.CreateScope();

                        try
                        {
                            var action = (IAction)scope.ServiceProvider.GetRequiredKeyedService(typeof(IAction), actionDetails.Name);
                            await InvokeActionAsync(action, resolved, ct);
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine($"[WARN] No action service registered for '{actionDetails.Name}' in trigger '{trig.Name}'.");
                            continue;
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
    /// Invokes an action with string parameters (for testing/manual execution).
    /// </summary>
    /// <param name="action">The action to invoke</param>
    /// <param name="parameterMap">String parameter map</param>
    /// <param name="ct">Cancellation token</param>
    public static Task InvokeActionAsync(IAction action, IReadOnlyDictionary<string, string> parameterMap, CancellationToken ct)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var kvp in parameterMap)
        {
            dict[kvp.Key] = kvp.Value;
        }
        return InvokeActionAsync(action, new ReadOnlyDictionary<string, object?>(dict), ct);
    }

    private static async Task InvokeActionAsync(IAction action, IReadOnlyDictionary<string, object?> parameters, CancellationToken ct)
    {
        // Find the IAction<TParameters> interface to get the parameter type
        var actionType = action.GetType();
        var genericInterface = actionType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAction<>));

        if (genericInterface == null)
        {
            throw new InvalidOperationException($"Action {actionType.Name} does not implement IAction<TParameters>");
        }

        var parameterType = genericInterface.GetGenericArguments()[0];

        // Convert the dictionary to strongly-typed parameters
        var typedParameters = ConvertToTypedParameters(parameters, parameterType);

        // Invoke ExecuteAsync using reflection
        var executeMethod = genericInterface.GetMethod("ExecuteAsync");
        if (executeMethod == null)
        {
            throw new InvalidOperationException($"Could not find ExecuteAsync method on {actionType.Name}");
        }

        var task = executeMethod.Invoke(action, new object[] { typedParameters, ct });
        if (task is Task t)
        {
            await t;
        }
    }

    private static object ConvertToTypedParameters(IReadOnlyDictionary<string, object?> parameters, Type parameterType)
    {
        // Serialize to JSON and deserialize to the target type
        // This handles property name mapping and type conversion
        var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var result = JsonSerializer.Deserialize(json, parameterType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        });

        if (result == null)
        {
            throw new InvalidOperationException($"Failed to deserialize parameters to {parameterType.Name}");
        }

        return result;
    }
}