using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
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
            return await result.Content.ReadAsAsync<T>();
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
                            default(T) :
                            JsonConvert.DeserializeObject<T>(data);
        }


        public static async Task<string> AuthenticateAsAdminUser(this HttpClient client)
        {
            var request = new AuthenticateUserRequest(AdminUser.UserName, AdminUser.DefaultPassword);
            var response = await client.PostAsync("api/users/authenticate", new JsonContent(request));
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
            return  await client.SendAsync(httpRequest);
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
    }
}