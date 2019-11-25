using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Users;

using Newtonsoft.Json;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string requestUri)
        {
            var result = await client.GetAsync(requestUri);
            result.EnsureSuccessStatusCode();
            return await result.ContentAs<T>();
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string requestUri, string bearerToken, T content)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri),
                Content = new JsonContent(content)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            return await client.SendAsync(request);
        }

        public static async Task<T> ContentAs<T>(this HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(data) ?
                            default :
                            JsonConvert.DeserializeObject<T>(data);
        }


        public static async Task<string> AuthenticateAsAdminUser(this HttpClient client)
        {
            var response = await client.AuthenticateUser(TestData.AuthenticateAdminUserRequest);
            var content = await response.ContentAs<AuthenticateUserResponse>();
            return content.Token;
        }

        public static async Task<HttpResponseMessage> CreateUser(this HttpClient client, RegisterUserRequest createUserRequest)
        {
            var token = await client.AuthenticateAsAdminUser();
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users")
                .AddBearerToken(token)
                .AddContent(new JsonContent(createUserRequest))
                .Build();
            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> AuthenticateUser(this HttpClient client, AuthenticateUserRequest authenticateUserRequest)
        {
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users/authenticate")
                .AddContent(new JsonContent(authenticateUserRequest))
                .Build();
            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> GetAllUsers(this HttpClient client)
        {
            var token = await client.AuthenticateAsAdminUser();

            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/users")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> AddUserToLocation(this HttpClient client, AddUserLocationRequest addUserLocationRequest)
        {
            var token = await client.AuthenticateAsAdminUser();

            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/locations/users")
                .AddJsonContent(addUserLocationRequest)
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(postRequest);
        }


        public static async Task<HttpResponseMessage> PostAuthenticateRequest(this HttpClient client, string userName, string password)
        {
            var authenticateRequest = new HttpRequestBuilder()
                .AddJsonContent(new AuthenticateUserRequest(userName, password))
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users/authenticate")
                .Build();
            return await client.SendAsync(authenticateRequest);
        }

        public static async Task<HttpResponseMessage> CreateLocation(this HttpClient client, CreateLocationRequest request)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddContent(new JsonContent(request))
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/locations")
                .Build();
            return await client.SendAsync(httpRequest);
        }

        public static async Task<GetLocationsResponse[]> GetLocations(this HttpClient client)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/locations")
                .Build();
            var response = await client.SendAsync(httpRequest);
            return await response.ContentAs<GetLocationsResponse[]>();
        }


        public static async Task<HttpResponseMessage> CreateZone(this HttpClient client, long locationId, CreateZoneRequest request)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(request)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri($"api/locations/{locationId}/zones")
                .Build();
            return await client.SendAsync(httpRequest);
        }


        public static async Task<HttpResponseMessage> GetZones(this HttpClient client, long locationId)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Get)
                .AddRequestUri($"api/locations/{locationId}/zones")
                .Build();
            return await client.SendAsync(httpRequest);
        }

        public static async Task<string> GetApiKey(this HttpClient client, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/users/apikey")
                .AddBearerToken(token)
                .Build();
            var responseMessage = await client.SendAsync(request);
            var content = await responseMessage.ContentAs<GetApiKeyResponse>();
            return content.ApiKey;
        }

        public static async Task<HttpResponseMessage> CreateMeasurement(this HttpClient client, CreateMeasurementRequest[] requests, string token)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(requests)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri($"api/measurements")
                .Build();
            return await client.SendAsync(httpRequest);
        }
    }
}
