using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Helper class to build ActionDetails from IAction&lt;TParameters&gt; types using reflection.
/// </summary>
public static class ActionDetailsBuilder
{
    /// <summary>
    /// Builds an ActionDetails from an action type that implements IAction&lt;TParameters&gt;.
    /// Uses the type name as the action name, DisplayName attribute for display name,
    /// and reflects over TParameters properties to build the parameter schema.
    /// </summary>
    /// <param name="actionType">The action type</param>
    /// <returns>An ActionDetails describing the action</returns>
    public static ActionDetails BuildFrom(Type actionType)
    {
        if (!typeof(IAction).IsAssignableFrom(actionType))
        {
            throw new ArgumentException($"Type {actionType.Name} does not implement IAction", nameof(actionType));
        }

        // Find the IAction<TParameters> interface
        var genericInterface = actionType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAction<>));

        if (genericInterface == null)
        {
            throw new ArgumentException($"Type {actionType.Name} does not implement IAction<TParameters>", nameof(actionType));
        }

        var parameterType = genericInterface.GetGenericArguments()[0];

        // Get the ActionAttribute
        var actionAttr = actionType.GetCustomAttribute<ActionAttribute>();
        if (actionAttr == null)
        {
            throw new ArgumentException($"Type {actionType.Name} must have an [Action] attribute", nameof(actionType));
        }

        // Get the action name from the type name (remove "Action" suffix if present)
        var name = actionType.Name.EndsWith("Action", StringComparison.OrdinalIgnoreCase)
            ? actionType.Name[..^6]
            : actionType.Name;

        // Build parameter schema from TParameters properties
        var parameterSchema = BuildParameterSchema(parameterType);

        return new ActionDetails
        {
            Id = actionAttr.Id,
            Name = name,
            DisplayName = actionAttr.Name,
            Description = actionAttr.Description,
            ParameterSchema = parameterSchema
        };
    }

    private static IReadOnlyList<ActionParameter> BuildParameterSchema(Type parameterType)
    {
        var properties = parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var parameters = new List<ActionParameter>();

        foreach (var prop in properties)
        {
            var name = prop.Name;
            var type = GetTypeString(prop.PropertyType);
            var required = IsRequired(prop);
            var description = GetDescription(prop);

            parameters.Add(new ActionParameter(name, type, required, description));
        }

        return parameters;
    }

    private static string GetTypeString(Type type)
    {
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.Name.ToLowerInvariant() switch
        {
            "string" => "string",
            "int32" or "int64" or "int16" or "byte" or "decimal" or "double" or "float" => "number",
            "boolean" => "boolean",
            _ => "string" // Default to string for unknown types
        };
    }

    private static bool IsRequired(PropertyInfo property)
    {
        // Check for Required attribute
        if (property.GetCustomAttribute<RequiredAttribute>() != null)
            return true;

        // Non-nullable value types are required (unless Nullable<T>)
        if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
            return true;

        // Reference types with nullable annotations - check for nullable reference type
        var nullabilityContext = new NullabilityInfoContext();
        var nullability = nullabilityContext.Create(property);
        if (nullability.WriteState == NullabilityState.NotNull)
            return true;

        return false;
    }

    private static string? GetDescription(PropertyInfo property)
    {
        var descriptionAttr = property.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttr?.Description;
    }
}
