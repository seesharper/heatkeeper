namespace HeatKeeper.Server.Events;

/// <summary>
/// Describes a parameter for an action, including its type, whether it's required, and optional description.
/// </summary>
/// <param name="Name">The parameter name</param>
/// <param name="Type">The parameter type (e.g. "string", "number", "boolean")</param>
/// <param name="Required">Whether this parameter is required</param>
/// <param name="Description">Optional description of the parameter</param>
public sealed record ActionParameter(string Name, string Type, bool Required, string? Description = null);