using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace HeatKeeper.Server.Http;

/// <summary>
/// A builder class that can be used to build a <see cref="HttpRequestMessage"/> with a fluent interface. 
/// </summary>
public class HttpRequestBuilder
{
    private string _requestUri = string.Empty;

    private UriKind _uriKind = UriKind.Relative;

    private const string DefaultAcceptHeader = "application/json";

    private readonly HttpRequestMessage _requestMessage = new();

    private readonly Dictionary<string, string> _queryParameters = [];

    /// <summary>
    /// Sets the <see cref="HttpMethod"/> of the request. Defaults to <see cref="HttpMethod.Get"/>.
    /// </summary>
    /// <param name="method">The <see cref="HttpMethod"/> to be used for this request.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for chaining calls.</returns>
    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        _requestMessage.Method = method;
        return this;
    }

    /// <summary>
    /// Sets the request uri. This will be appended to the base url.
    /// </summary>
    /// <param name="requestUri">The uri of the request</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for chaining calls.</returns>
    public HttpRequestBuilder WithRequestUri(string requestUri)
    {
        _requestUri = requestUri;
        return this;
    }

    /// <summary>
    /// Sets the request <see cref="UriKind"/>.
    /// </summary>
    /// <remarks>Default is <see cref="UriKind.Relative"/></remarks>
    /// <param name="uriKind">Specify if the URI is relative or absolute.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for chaining calls.</returns>
    public HttpRequestBuilder WithUriKind(UriKind uriKind)
    {
        _uriKind = uriKind;
        return this;
    }

    /// <summary>
    /// Adds a query parameter to the request uri.
    /// </summary>
    /// <typeparam name="T">The type of value to be added.</typeparam>
    /// <param name="name">The name of he query parameter.</param>
    /// <param name="value">The query parameter value.</param>
    /// <returns>The <see cref="HttpRequestBuilder"/> for chaining calls.</returns>
    /// <exception cref="ArgumentException">Thrown if the string representation (ToString()) is null.</exception>
    public HttpRequestBuilder AddQueryParameter<T>(string name, T value) where T : notnull
    {
        string? stringValue = value.ToString();
        if (stringValue is null)
        {
            throw new ArgumentException($"The parameter {nameof(value)} must return an non-null value from its `ToString()` method.");
        }
        _queryParameters[name] = stringValue.ToString();
        return this;
    }

    /// <summary>
    /// Adds a list of query parameters to the request uri.
    /// </summary>
    /// <typeparam name="T">The type of the values to be added.</typeparam>
    /// <param name="name">The name of the query parameter.</param>
    /// <param name="values">A list of values </param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpRequestBuilder AddQueryParameter<T>(string name, params T[] values) where T : notnull
    {
        var result = new StringBuilder();
        foreach (var item in values)
        {
            string? stringValue = item.ToString();
            if (stringValue is null)
            {
                throw new ArgumentException($"The parameter {nameof(values)} must contain elements where all return a non-null value from its `ToString()` method.");
            }

            result.Append(item.ToString());
            result.Append(',');
        }
        result.Length--; // remove the last comma
        _queryParameters[name] = result.ToString();
        return this;
    }

    /// <summary>
    /// Adds a header to the request.
    /// </summary>
    /// <param name="name">The name of the header.</param>
    /// <param name="value">The header value.</param>
    /// <returns></returns>
    public HttpRequestBuilder AddHeader(string name, string value)
    {
        _requestMessage.Headers.Add(name, value);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="acceptHeader"></param>
    /// <returns></returns>
    public HttpRequestBuilder AddAcceptHeader(string acceptHeader)
    {
        _requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        return this;
    }

    /// <summary>
    /// Sets the request message content to JSON serialized from the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize to JSON.</typeparam>
    /// <param name="content">The object to serialize to JSON and use as the request message content.</param>
    /// <param name="mediaType">The media type of the JSON content. Defaults to 'application/json'.</param>
    /// <param name="options">The JSON serialization options to use.</param>
    /// <returns>A reference to this instance after the request message content has been set.</returns>
    public HttpRequestBuilder WithJsonContent<T>(T content, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null) where T : class
    {
        _requestMessage.Content = JsonContent.Create(content, mediaType, options);
        return this;
    }

    /// <summary>
    /// /// Sets the request message content to JSON serialized from the specified object.
    /// </summary>
    /// <param name="contentValue">The object to serialize to JSON and use as the request message content.</param>
    /// <param name="contentType">The <see cref="Type"/> of the request message content.</param>
    /// <param name="mediaType">The media type of the JSON content. Defaults to 'application/json'.</param>
    /// <param name="options">The JSON serialization options to use.</param>
    /// <returns>A reference to this instance after the request message content has been set.</returns>
    public HttpRequestBuilder WithJsonContent(object contentValue, Type contentType, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
    {
        _requestMessage.Content = JsonContent.Create(contentValue, contentType, mediaType, options);
        return this;
    }

    /// <summary>
    /// Sets the request message content to the specified <see cref="HttpContent"/> instance.
    /// </summary>
    /// <param name="content">The <see cref="HttpContent"/> to be set as the request message content.</param>
    /// <returns>A reference to this instance after the request message content has been set.</returns>
    public HttpRequestBuilder WithContent(HttpContent content)
    {
        _requestMessage.Content = content;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="HttpRequestMessage"/> instance using the configured settings.
    /// </summary>
    /// <returns>The configured <see cref="HttpRequestMessage"/>.</returns>
    public HttpRequestMessage Build()
    {
        var queryString = BuildQueryString();
        if (!string.IsNullOrWhiteSpace(queryString))
        {
            _requestUri = $"{_requestUri}?{queryString}";
        }

        if (_requestMessage.Headers.Accept.Count == 0)
        {
            AddAcceptHeader(DefaultAcceptHeader);
        }

        _requestMessage.RequestUri = new Uri(_requestUri, _uriKind);
        return _requestMessage;
    }

    private string BuildQueryString()
    {
        if (_queryParameters.Count == 0)
        {
            return string.Empty;
        }
        var encoder = UrlEncoder.Default;
        return _queryParameters
            .Select(kvp => $"{encoder.Encode(kvp.Key)}={encoder.Encode(kvp.Value)}")
            .Aggregate((current, next) => $"{current}&{next}");
    }
}
