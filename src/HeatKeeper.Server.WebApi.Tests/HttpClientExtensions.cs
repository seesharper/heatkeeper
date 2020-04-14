using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
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
            var content = await response.ContentAs<AuthenticatedUserQueryResult>();
            return content.Token;
        }

        public static async Task<HttpResponseMessage> RegisterUser(this HttpClient client, RegisterUserCommand command, string token)
        {
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users")
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .Build();
            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> DeleteUser(this HttpClient client, DeleteUserCommand command, string token)
        {
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Delete)
                .AddRequestUri($"api/users/{command.UserId}")
                .AddBearerToken(token)
                .Build();
            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> AuthenticateUser(this HttpClient client, AuthenticatedUserQuery query)
        {
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users/authenticate")
                .AddContent(new JsonContent(query))
                .Build();
            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> GetAllUsers(this HttpClient client, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/users")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> GetUsersByLocation(this HttpClient client, long locationId)
        {
            var token = await client.AuthenticateAsAdminUser();

            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri($"api/locations/{locationId}/users")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> AddUserToLocation(this HttpClient client, long locationId, AddUserToLocationCommand addUserLocationRequest, string token)
        {
            var postRequest = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri($"api/locations/{locationId}/users")
                .AddJsonContent(addUserLocationRequest)
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(postRequest);
        }

        public static async Task<HttpResponseMessage> RemoveUserFromRequest(this HttpClient client, long locationId, long userID)
        {
            var token = await client.AuthenticateAsAdminUser();

            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Delete)
                .AddRequestUri($"api/locations/{locationId}/users/{userID}")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(request);
        }

        public static async Task<string> CreateAndAuthenticateStandardUser(this HttpClient client)
        {
            var token = await client.AuthenticateAsAdminUser();

            var registerUserRequest = TestData.Users.StandardUser;
            var registerUserResponse = await client.RegisterUser(registerUserRequest, token);
            registerUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var authenticateResponse = await client.PostAuthenticateRequest(registerUserRequest.Email, registerUserRequest.Password);
            authenticateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await authenticateResponse.ContentAs<AuthenticatedUserQueryResult>();
            return content.Token;

        }

        public static async Task<HttpResponseMessage> PostAuthenticateRequest(this HttpClient client, string email, string password)
        {
            var authenticateRequest = new HttpRequestBuilder()
                .AddJsonContent(new AuthenticatedUserQuery(email, password))
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/users/authenticate")
                .Build();
            return await client.SendAsync(authenticateRequest);
        }

        public static async Task<HttpResponseMessage> CreateLocation(this HttpClient client, CreateLocationCommand command, string token)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .WithMethod(HttpMethod.Post)
                .AddRequestUri("api/locations")
                .Build();
            return await client.SendAsync(httpRequest);
        }

        public static async Task<HttpResponseMessage> PatchLocation(this HttpClient client, UpdateLocationCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Patch)
                .AddRequestUri($"api/locations/{command.LocationId}")
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .Build();
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PatchZone(this HttpClient client, UpdateZoneCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Patch)
                .AddRequestUri($"api/zones/{command.ZoneId}")
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .Build();
            return await client.SendAsync(request);
        }

        public static async Task<Location[]> GetLocations(this HttpClient client, string token)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/locations")
                .Build();
            var response = await client.SendAsync(httpRequest);
            return await response.ContentAs<Location[]>();
        }


        public static async Task<HttpResponseMessage> CreateZone(this HttpClient client, long locationId, CreateZoneCommand request)
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


        public static async Task<Zone[]> GetZones(this HttpClient client, long locationId)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Get)
                .AddRequestUri($"api/locations/{locationId}/zones")
                .Build();
            var response = await client.SendAsync(httpRequest);
            return await response.ContentAs<Zone[]>();
        }

        public static async Task<HttpResponseMessage> GetZoneDetails(this HttpClient client, string token, long zoneId)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri($"api/zones/{zoneId}")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> GetSensors(this HttpClient client, string token, long zoneId)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri($"api/zones/{zoneId}/sensors")
                .AddBearerToken(token)
                .Build();

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> AddSensorToZone(this HttpClient client, AddSensorToZoneCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Post)
                .AddRequestUri($"api/zones/{command.ZoneId}/sensors")
                .AddJsonContent(command)
                .AddBearerToken(token)
                .Build();
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> RemoveSensorFromZone(this HttpClient client, RemoveSensorFromZoneCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Delete)
                .AddRequestUri($"api/zones/{command.ZoneId}/sensors")
                .AddJsonContent(command)
                .AddBearerToken(token)
                .Build();
            return await client.SendAsync(request);
        }


        public static async Task<string> GetApiKey(this HttpClient client, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .AddRequestUri("api/users/apikey")
                .AddBearerToken(token)
                .Build();
            var responseMessage = await client.SendAsync(request);
            var content = await responseMessage.ContentAs<ApiKey>();
            return content.Token;
        }

        public static async Task<HttpResponseMessage> CreateMeasurement(this HttpClient client, CreateMeasurementCommand[] requests, string token)
        {
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(requests)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri($"api/measurements")
                .Build();
            return await client.SendAsync(httpRequest);
        }

        public static async Task<HttpResponseMessage> PatchUser(this HttpClient client, UpdateUserCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Patch)
                .AddRequestUri($"api/users/{command.UserId}")
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .Build();
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PatchCurrentUser(this HttpClient client, UpdateCurrentUserCommand command, string token)
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Patch)
                .AddRequestUri($"api/users")
                .AddBearerToken(token)
                .AddContent(new JsonContent(command))
                .Build();
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> ChangePassword(this HttpClient client, ChangePasswordCommand command, string token)
        {
            var request = new HttpRequestBuilder()
               .WithMethod(HttpMethod.Patch)
               .AddRequestUri("api/users/password")
               .AddBearerToken(token)
               .AddContent(new JsonContent(command))
               .Build();
            return await client.SendAsync(request);
        }
    }
}
