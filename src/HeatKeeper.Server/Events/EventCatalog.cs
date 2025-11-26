using System.Reflection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Metadata about an event type and its payload properties.
/// </summary>
/// <param name="Id">Unique identifier for the event type</param>
/// <param name="Name">Human-readable name for the event</param>
/// <param name="Description">Description of what this event represents</param>
/// <param name="EventType">The event type name (derived from payload type name)</param>
/// <param name="Properties">List of properties in the payload</param>
public sealed record EventDetails(
    int Id,
    string Name,
    string Description,
    string EventType,
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
    private readonly Dictionary<int, EventDetails> _eventTypes = new();
    private readonly HashSet<Assembly> _scannedAssemblies = new();
    private readonly object _lock = new();

    /// <summary>
    /// Scans the specified assembly for types that can be used as DomainEvent payloads.
    /// </summary>
    /// <param name="assembly">The assembly to scan. If null, scans the calling assembly.</param>
    public void ScanAssembly(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        lock (_lock)
        {
            // Skip if this assembly has already been scanned
            if (_scannedAssemblies.Contains(assembly))
            {
                return;
            }

            var payloadTypes = assembly.GetTypes()
                .Where(IsValidPayloadType)
                .Where(type => type.GetCustomAttribute<DomainEventAttribute>() != null) // Only types with DomainEventAttribute
                .ToList();

            foreach (var payloadType in payloadTypes)
            {
                var eventType = payloadType.Name; // This matches EventBus.PublishAsync logic

                // Extract metadata from DomainEventAttribute (we know it exists now)
                var domainEventAttribute = payloadType.GetCustomAttribute<DomainEventAttribute>()!;
                var id = domainEventAttribute.Id;
                var name = domainEventAttribute.Name;
                var description = domainEventAttribute.Description;

                // Check if this event is already registered to avoid duplicates
                if (_eventTypes.ContainsKey(id))
                {
                    continue;
                }

                var properties = GetPayloadProperties(payloadType);
                _eventTypes.Add(id, new EventDetails(id, name, description, eventType, properties));
            }

            // Mark this assembly as scanned
            _scannedAssemblies.Add(assembly);
        }
    }

    /// <summary>
    /// Gets all discovered event types.
    /// </summary>
    /// <returns>A read-only list of event type information</returns>
    public IReadOnlyList<EventDetails> ListEventTypes()
    {
        lock (_lock)
        {
            return _eventTypes.Values.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Gets event details by event ID.
    /// </summary>
    /// <param name="id">The event ID</param>
    /// <returns>Event details if found</returns>
    public EventDetails GetEventDetails(int id)
    {
        lock (_lock)
        {
            return _eventTypes[id]; // Throws KeyNotFoundException if not found
        }
    }

    /// <summary>
    /// Clears all discovered event types.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _eventTypes.Clear();
            _scannedAssemblies.Clear();
        }
    }

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