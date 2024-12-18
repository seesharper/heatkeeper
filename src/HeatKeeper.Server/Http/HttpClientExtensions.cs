using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Http;

/// <summary>
/// Extends the <see cref="HttpClient"/> class.
/// </summary>
public static class HttpClientExtensions
{

    /// <summary>
    /// Sends an HTTP request using the provided <see cref="HttpClient"/> instance, and handles any errors or problem details returned by the server.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending the request.</param>
    /// <param name="httpRequest">The <see cref="HttpRequestMessage"/> to send.</param>
    /// <param name="isSuccessful">An optional function that determines whether the HTTP response is considered successful. By default, any response with a successful status code (2xx) is considered successful.</param>
    /// <param name="problemHandler">An optional action to handle any <see cref="ProblemDetails"/> objects returned by the server. By default, a <see cref="ProblemDetailsException"/> is thrown.</param>
    /// <param name="errorHandler">An optional function to handle any other errors or exceptions returned by the server. By default, a generic <see cref="HttpRequestException"/> is thrown with the raw response content.</param>
    /// <param name="cancellationToken">An optional cancellation token that will be forwarded to the SendAsync method.</param>
    /// <returns>The <see cref="HttpResponseMessage"/> returned by the server.</returns>
    /// <exception cref="ProblemDetailsException">Thrown if the server returns a <see cref="ProblemDetails"/> response.</exception>
    /// <exception cref="HttpRequestException">Thrown if the server returns a non-successful response and no <see cref="ProblemDetails"/> response.</exception>
    public static async Task<HttpResponseMessage> SendAndHandleRequest(this HttpClient client, HttpRequestMessage httpRequest, Func<HttpResponseMessage, bool>? isSuccessful = null, Action<ProblemDetails>? problemHandler = null, Func<HttpResponseMessage, Task>? errorHandler = null, CancellationToken cancellationToken = default)
    {
        isSuccessful ??= (responseMessage) => responseMessage.IsSuccessStatusCode;
        problemHandler ??= (problemDetails) => ReadProblemDetailsAndThrowException(httpRequest, problemDetails);
        errorHandler ??= async (responseMessage) => await ReadAndThrowException(responseMessage);

        var response = await client.SendAsync(httpRequest, cancellationToken);

        if (!isSuccessful(response))
        {
            ProblemDetails? problemDetails = await ReadProblemDetails(response);
            if (problemDetails is not null)
            {
                problemHandler(problemDetails);
            }
            else
            {
                await errorHandler(response);
            }
        }

        return response;
    }

    private static Action<ProblemDetails> ReadProblemDetailsAndThrowException(HttpRequestMessage httpRequest, ProblemDetails problemDetails)
    {
        StringBuilder message = new();

        message.AppendLine($"There was a problem handling the request");
        message.AppendLine(httpRequest.ToString());
        if (problemDetails.Status is not null)
        {
            message.AppendLine($"Status Code: {problemDetails.Status} ({(HttpStatusCode)problemDetails.Status})");
        }

        message.AppendLine($"Title: {problemDetails.Title}");
        message.AppendLine($"Detail: {problemDetails.Detail}");
        if (problemDetails.Extensions.Count > 0)
        {
            message.AppendLine("The following extensions were found and could possibly provide more information about the problem: ");
            foreach (var extension in problemDetails.Extensions)
            {
                message.AppendLine($"{extension.Key} : {extension.Value}");
            }
        }

        throw new ProblemDetailsException(httpRequest!.RequestUri!.ToString(), problemDetails.Title, problemDetails.Detail, message.ToString(), problemDetails.Status, problemDetails.Extensions);
    }

    private static async Task ReadAndThrowException(HttpResponseMessage response)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        string message =
        $"""
        There was a problem handling the request
        {response.RequestMessage!}
        The response was
        {response}
        The raw string response was
        {stringResponse}
        """;
        throw new HttpRequestException(message, null, response.StatusCode);
    }

    private static async Task<ProblemDetails?> ReadProblemDetails(HttpResponseMessage responseMessage)
    {
        try
        {
            return await responseMessage.Content.ReadFromJsonAsync<ProblemDetails>();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
