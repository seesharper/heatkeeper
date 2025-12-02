#nullable enable
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Resolves property placeholders in parameter values using the pattern {{propertyName}}.
/// This allows dynamic binding of domain event properties to action parameters.
/// </summary>
public static class PropertyResolver
{
    private static readonly Regex PlaceholderPattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

    /// <summary>
    /// Resolves placeholders in a parameter value by replacing {{propertyName}} with actual values from the event payload.
    /// </summary>
    /// <param name="parameterValue">The parameter value that may contain placeholders like {{ZoneId}}</param>
    /// <param name="payload">The event payload object to read property values from</param>
    /// <returns>The resolved value with placeholders replaced, or the original value if no placeholders found</returns>
    public static object? ResolveValue(string parameterValue, object payload)
    {
        if (string.IsNullOrEmpty(parameterValue))
            return parameterValue;

        var matches = PlaceholderPattern.Matches(parameterValue);
        if (matches.Count == 0)
            return parameterValue;

        // If the entire string is a single placeholder, return the actual typed value
        if (matches.Count == 1 && matches[0].Value == parameterValue)
        {
            var propertyName = matches[0].Groups[1].Value;
            return ReadPropertyValue(payload, propertyName);
        }

        // If there are multiple placeholders or mixed content, perform string substitution
        var result = parameterValue;
        foreach (Match match in matches)
        {
            var propertyName = match.Groups[1].Value;
            var propertyValue = ReadPropertyValue(payload, propertyName);
            var stringValue = ConvertToString(propertyValue);
            result = result.Replace(match.Value, stringValue);
        }

        return result;
    }

    /// <summary>
    /// Reads a property value from the payload object using reflection.
    /// </summary>
    /// <param name="payload">The object to read the property from</param>
    /// <param name="propertyName">The name of the property (case-insensitive)</param>
    /// <returns>The property value or null if not found</returns>
    private static object? ReadPropertyValue(object payload, string propertyName)
    {
        var payloadType = payload.GetType();
        var property = payloadType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property == null)
        {
            Console.WriteLine($"[WARN] Property '{propertyName}' not found on payload type '{payloadType.Name}'");
            return null;
        }

        return property.GetValue(payload);
    }

    /// <summary>
    /// Converts a property value to its string representation.
    /// </summary>
    private static string ConvertToString(object? value)
    {
        return value switch
        {
            null => string.Empty,
            JsonElement je => je.ToString(),
            _ => value.ToString() ?? string.Empty
        };
    }
}
