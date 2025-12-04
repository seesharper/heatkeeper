#nullable enable

namespace HeatKeeper.Server.Events;

/// <summary>
/// Describes a parameter for an action, including its type, whether it's required, and optional description.
/// </summary>
/// <param name="Name">The parameter name</param>
/// <param name="Type">The parameter type (e.g. "string", "number", "boolean")</param>
/// <param name="Required">Whether this parameter is required</param>
/// <param name="Description">Optional description of the parameter</param>
/// <param name="LookupUrl">Optional API URL for lookup options (e.g., "api/locations")</param>
public sealed record ActionParameter(string Name, string Type, bool Required, string? Description = null, string? LookupUrl = null);