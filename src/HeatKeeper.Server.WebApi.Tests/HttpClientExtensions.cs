using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Dashboard;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Version;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ContentAs<T>(this HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(data) ?
                            default :
                            JsonConvert.DeserializeObject<T>(data);
        }

        public static async Task<string> AuthenticateAsAdminUser(this HttpClient client)
        {
            var authenticatedUser = await client.GetAuthenticatedUser(
                TestData.AuthenticateAdminUserRequest,
                success: response => response.StatusCode.ShouldBeOK());
            return authenticatedUser.Token;
        }

        public static async Task<long> CreateUser(this HttpClient client, RegisterUserCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/users", content, token, success, problem);

        public static async Task DeleteZone(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/zones/{zoneId}", token, success, problem);

        public static async Task Delete(HttpClient client, string uri, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Delete)
                .AddRequestUri(uri)
                .Build();

            await SendAndHandleRequest(client, success, problem, httpRequest);
        }

        public static async Task DeleteUser(this HttpClient client, long userId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/users/{userId}", token, success, problem);

        public static async Task<AuthenticatedUser> GetAuthenticatedUser(this HttpClient client, AuthenticatedUserQuery query, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post<AuthenticatedUserQuery, AuthenticatedUser>(client, "api/users/authenticate", query, string.Empty, success, problem);

        public static async Task<User[]> GetAllUsers(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<User[]>(client, "api/users", token, success, problem);

        public static async Task<User[]> GetUsersByLocation(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<User[]>(client, $"api/locations/{locationId}/users", token, success, problem);

        public static async Task<AppVersion> GetAppVersion(this HttpClient client)
            => await Get<AppVersion>(client, "api/version", string.Empty);

        public static async Task<DashboardLocation[]> GetDashboardLocations(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<DashboardLocation[]>(client, "api/dashboard/locations", token, success, problem);

        public static async Task AddUserToLocation(this HttpClient client, long locationId, AddUserToLocationCommand addUserLocationRequest, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            await PostWithNoResponse(client, $"api/locations/{locationId}/users", addUserLocationRequest, token, success, problem);
        }

        public static async Task RemoveUserFromLocation(this HttpClient client, long locationId, long userID, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/locations/{locationId}/users/{userID}", token, success, problem);

        public static async Task<string> CreateAndAuthenticateStandardUser(this HttpClient client)
        {
            var token = await client.AuthenticateAsAdminUser();

            var registerUserRequest = TestData.Users.StandardUser;
            await client.CreateUser(registerUserRequest, token);
            var authenticateResponse = await client.PostAuthenticateRequest(registerUserRequest.Email, registerUserRequest.Password);
            authenticateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await authenticateResponse.ContentAs<AuthenticatedUser>();
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

        public static async Task<long> CreateLocation(this HttpClient client, CreateLocationCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/locations", content, token, success, problem);

        public static async Task UpdateLocation(this HttpClient client, UpdateLocationCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/locations/{content.Id}", content, token, success, problem);

        public static async Task UpdateZone(this HttpClient client, UpdateZoneCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/zones/{content.ZoneId}", content, token, success, problem);

        public static async Task<Location[]> GetLocations(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Location[]>(client, "api/locations", token, success, problem);

        private static async Task<TContent> Get<TContent>(HttpClient client, string uri, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.OK);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Get)
                .AddRequestUri(uri)
                .Build();
            var response = await SendAndHandleRequest(client, success, problem, httpRequest);
            return await response.ContentAs<TContent>();
        }

        private static async Task<long> Post<TContent>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            var resourceId = await Post<TContent, ResourceId>(client, uri, content, token, success, problem);
            return resourceId.Id;
        }

        private static async Task<TResponse> Post<TContent, TResponse>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.Created);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(content)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri(uri)
                .Build();

            var response = await SendAndHandleRequest(client, success, problem, httpRequest);
            return await response.ContentAs<TResponse>();
        }

        private static async Task PostWithNoResponse<TContent>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.OK);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(content)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri(uri)
                .Build();

            await SendAndHandleRequest(client, success, problem, httpRequest);
        }

        private static async Task<HttpResponseMessage> SendAndHandleRequest(HttpClient client, Action<HttpResponseMessage> success, Action<ProblemDetails> problem, HttpRequestMessage httpRequest)
        {
            var response = await client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                success(response);
            }
            else
            {
                problem.Should().NotBeNull($"There was a problem handling the request and this was not not handled in the calling test method. The status code was ({(int)response.StatusCode}) {response.StatusCode}");
                var problemDetails = await response.ContentAs<ProblemDetails>();
                problem(problemDetails);
            }

            return response;
        }

        private static async Task Patch<TContent>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.OK);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddJsonContent(content)
                .WithMethod(HttpMethod.Patch)
                .AddRequestUri(uri)
                .Build();

            await SendAndHandleRequest(client, success, problem, httpRequest);
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

        public static async Task<long> CreateZone(this HttpClient client, long locationId, CreateZoneCommand request, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/locations/{locationId}/zones", request, token, success, problem);

        public static async Task DeleteLocation(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/locations/{locationId}", token, success, problem);

        public static async Task<Zone[]> GetZones(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Zone[]>(client, $"api/locations/{locationId}/zones", token, success, problem);

        public static async Task<ZoneDetails> GetZoneDetails(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneDetails>(client, $"api/zones/{zoneId}", token, success, problem);

        public static async Task<Sensor[]> GetSensors(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Sensor[]>(client, $"api/zones/{zoneId}/sensors", token, success, problem);

        public static async Task AddSensorToZone(this HttpClient client, long zoneId, AddSensorToZoneCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await PostWithNoResponse(client, $"api/zones/{zoneId}/sensors", content, token, success, problem);

        public static async Task<ApiKey> GetApiKey(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ApiKey>(client, "api/users/apikey", token, success, problem);

        public static async Task CreateMeasurement(this HttpClient client, MeasurementCommand[] requests, string token)
            => await PostWithNoResponse(client, "api/measurements/", requests, token);

        public static async Task UpdateUser(this HttpClient client, UpdateUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/users/{command.UserId}", command, token, success, problem);

        public static async Task UpdateCurrentUser(this HttpClient client, UpdateCurrentUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, $"api/users", command, token, success, problem);

        public static async Task ChangePassword(this HttpClient client, ChangePasswordCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, "api/users/password", command, token, success, problem);
    }
}
