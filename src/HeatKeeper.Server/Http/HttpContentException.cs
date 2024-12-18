namespace HeatKeeper.Server.Http;

/// <summary>
/// Thrown when an error occurs while reading the content of an <see cref="HttpContent"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HttpContentException"/> class.
/// </remarks>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception</param>
public class HttpContentException(string message, Exception innerException) : Exception(message, innerException)
{
}