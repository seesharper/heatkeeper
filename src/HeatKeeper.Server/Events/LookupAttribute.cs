namespace HeatKeeper.Server.Events;

/// <summary>
/// Specifies a lookup URL for a property or parameter to provide a list of options for selection in the UI.
/// </summary>
/// <param name="url">The API URL that returns a list of options for this property (e.g., "api/locations")</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class LookupAttribute(string url) : Attribute
{
    /// <summary>
    /// Gets the API URL that provides lookup options for this property.
    /// </summary>
    public string Url { get; } = url;
}
