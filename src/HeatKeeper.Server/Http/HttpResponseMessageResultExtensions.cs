using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace HeatKeeper.Server.Http;

/// <summary>
/// Extends the <see cref="HttpResponseMessage"/> class.
/// </summary>
public static class HttpResponseMessageResultExtensions
{
    /// <summary>
    /// Reads the response content as JSON and deserializes it to an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON response content to.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/> to read the content from.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> to use.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns>The deserialized instance of the specified type.</returns>
    /// <exception cref="HttpContentException">Thrown if there was a problem deserializing the response content.</exception>
    public static async Task<T> ContentAs<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return (await response.Content.ReadFromJsonAsync<T>(options, cancellationToken))!;
        }
        catch (Exception ex)
        {
            var stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            string message =
            $"""
            There was a problem deserializing the content of the response into the specified type ({typeof(T)}).
            The raw string response was
            {stringResponse}
            """;
            throw new HttpContentException(message, ex);
        }
    }
}
