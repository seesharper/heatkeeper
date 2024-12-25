namespace HeatKeeper.Server.Http;

/// <summary>
/// Represents an exception that is thrown when a server returns a ProblemDetails response.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProblemDetailsException"/> class with the specified parameters.
/// </remarks>
/// <param name="requestUrl">The URL of the request that resulted in the problem details response.</param>
/// <param name="title">The problem details title.</param>
/// <param name="detail">The problem details detail.</param>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="status">The HTTP status code returned by the server.</param>
/// <param name="extensions">Additional key-value pairs associated with the problem details response.</param>
public class ProblemDetailsException(string? requestUrl, string? title, string? detail, string? message, int? status, IDictionary<string, object?> extensions) : Exception(message)
{

    /// <summary>
    /// Gets the URL of the request that resulted in the problem details response.
    /// </summary>
    public string? RequestUrl { get; } = requestUrl;

    /// <summary>
    /// Gets the problem details title.
    /// </summary>
    public string? Title { get; } = title;

    /// <summary>
    /// Gets the problem details detail.
    /// </summary>
    public string? Detail { get; } = detail;

    /// <summary>
    /// Gets the HTTP status code returned by the server.
    /// </summary>
    public int? Status { get; } = status;

    /// <summary>
    /// Gets additional key-value pairs associated with the problem details response.
    /// </summary>
    public IDictionary<string, object?> Extensions { get; } = extensions;
}
