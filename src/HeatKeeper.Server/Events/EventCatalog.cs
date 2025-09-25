using System.Reflection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Metadata about an event type and its payload properties.
/// </summary>
/// <param name="EventType">The event type name (derived from payload type name)</param>
/// <param name="PayloadType">The .NET type of the payload</param>
/// <param name="Properties">List of properties in the payload</param>
public sealed record EventTypeInfo(
    string EventType,
    Type PayloadType,
    IReadOnlyList<EventPropertyInfo> Properties
);

/// <summary>
/// Metadata about a property in an event payload.
/// </summary>
/// <param name="Name">The property name</param>
/// <param name="Type">The property type name (e.g., "int", "string", "double")</param>
/// <param name="IsNullable">Whether the property can be null</param>
/// <param name="Description">Optional description from XML documentation</param>
public sealed record EventPropertyInfo(
    string Name,
    string Type,
    bool IsNullable,
    string? Description = null
);

/// <summary>
/// Catalog for discovering and listing available event types by scanning assemblies.
/// </summary>
public sealed class EventCatalog : IEventCatalog
{
    private readonly List<EventTypeInfo> _eventTypes = new();

    /// <summary>
    /// Scans the specified assembly for types that can be used as DomainEvent payloads.
    /// </summary>
    /// <param name="assembly">The assembly to scan. If null, scans the calling assembly.</param>
    public void ScanAssembly(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var payloadTypes = assembly.GetTypes()
            .Where(IsValidPayloadType)
            .ToList();

        foreach (var payloadType in payloadTypes)
        {
            var eventType = payloadType.Name; // This matches DomainEvent<T>.EventType logic
            var properties = GetPayloadProperties(payloadType);

            _eventTypes.Add(new EventTypeInfo(eventType, payloadType, properties));
        }
    }

    /// <summary>
    /// Gets all discovered event types.
    /// </summary>
    /// <returns>A read-only list of event type information</returns>
    public IReadOnlyList<EventTypeInfo> ListEventTypes() => _eventTypes.AsReadOnly();

    /// <summary>
    /// Gets event type information by event type name.
    /// </summary>
    /// <param name="eventType">The event type name</param>
    /// <returns>Event type info if found, null otherwise</returns>
    public EventTypeInfo? GetEventType(string eventType)
        => _eventTypes.FirstOrDefault(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Clears all discovered event types.
    /// </summary>
    public void Clear() => _eventTypes.Clear();

    private static bool IsValidPayloadType(Type type)
    {
        // Look for types that would make good event payloads
        return type.IsPublic
            && !type.IsAbstract
            && !type.IsInterface
            && !type.IsGenericTypeDefinition
            && (type.IsClass || type.IsValueType)
            && type != typeof(string) // Exclude primitive types
            && type != typeof(object)
            && !type.IsPrimitive
            && !type.IsEnum
            && type.Namespace?.StartsWith("System") != true; // Exclude system types
    }

    private static IReadOnlyList<EventPropertyInfo> GetPayloadProperties(Type payloadType)
    {
        var properties = new List<EventPropertyInfo>();

        var publicProperties = payloadType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead) // Only readable properties
            .ToList();

        foreach (var prop in publicProperties)
        {
            var propertyType = prop.PropertyType;
            var isNullable = IsNullableType(propertyType);
            var typeName = GetFriendlyTypeName(propertyType);

            properties.Add(new EventPropertyInfo(
                Name: prop.Name,
                Type: typeName,
                IsNullable: isNullable,
                Description: null // Could be enhanced with XML documentation parsing
            ));
        }

        return properties.AsReadOnly();
    }

    private static bool IsNullableType(Type type)
    {
        if (!type.IsValueType) return true; // Reference types are nullable by default

        return Nullable.GetUnderlyingType(type) != null; // Nullable<T>
    }

    private static string GetFriendlyTypeName(Type type)
    {
        // Handle nullable value types
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return GetFriendlyTypeName(underlyingType) + "?";
        }

        // Handle common types with friendly names
        return type.Name switch
        {
            "String" => "string",
            "Int32" => "int",
            "Int64" => "long",
            "Double" => "double",
            "Single" => "float",
            "Decimal" => "decimal",
            "Boolean" => "bool",
            "DateTime" => "DateTime",
            "DateTimeOffset" => "DateTimeOffset",
            "Guid" => "Guid",
            _ => type.Name
        };
    }
}