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
using HeatKeeper.Server.Programs;
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

        public static async Task<DeadSensor[]> GetDeadSensors(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<DeadSensor[]>(client, "api/sensors/deadsensors", token, success, problem);

        public static async Task<User[]> GetUsersByLocation(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<User[]>(client, $"api/locations/{locationId}/users", token, success, problem);

        public static async Task<AppVersion> GetAppVersion(this HttpClient client)
            => await Get<AppVersion>(client, "api/version", string.Empty);

        public static async Task<DashboardLocation[]> GetDashboardLocations(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<DashboardLocation[]>(client, "api/dashboard/locations", token, success, problem);

        public static async Task<LocationDetails> GetLocationDetails(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<LocationDetails>(client, $"api/locations/{locationId}", token, success, problem);

        public static async Task<ProgramDetails> GetProgramDetails(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ProgramDetails>(client, $"api/programs/{programId}", token, success, problem);

        public static async Task<ScheduleDetails> GetScheduleDetails(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ScheduleDetails>(client, $"api/schedules/{scheduleId}", token, success, problem);

        public static async Task<ZoneInfo[]> GetZonesNotAssignedToSchedule(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneInfo[]>(client, $"api/schedules/{scheduleId}/zones", token, success, problem);

        public static async Task<SetPointDetails> GetSetPointDetails(this HttpClient client, long setPointId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<SetPointDetails>(client, $"api/setpoints/{setPointId}", token, success, problem);

        public static async Task<DashboardTemperature[]> GetDashboardTemperatures(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
         => await Get<DashboardTemperature[]>(client, "api/dashboard/temperatures", token, success, problem);

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

        public static async Task<long> CreateProgram(this HttpClient client, CreateProgramCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/locations/{content.LocationId}/programs", content, token, success, problem);

        public static async Task<long> CreateProgram(this HttpClient client, CreateLocationCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/programs", content, token, success, problem);

        public static async Task UpdateLocation(this HttpClient client, UpdateLocationCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/locations/{content.Id}", content, token, success, problem);

        public static async Task UpdateZone(this HttpClient client, UpdateZoneCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/zones/{content.ZoneId}", content, token, success, problem);

        public static async Task UpdateSetPoint(this HttpClient client, UpdateSetPointCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/setPoints/{content.SetPointId}", content, token, success, problem);

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

        private static async Task PostWithNoContent(HttpClient client, string uri, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.Created);

            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .WithMethod(HttpMethod.Post)
                .AddRequestUri(uri)
                .Build();

            var response = await SendAndHandleRequest(client, success, problem, httpRequest);
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

        public static async Task<long> CreateZone(this HttpClient client, long locationId, CreateZoneCommand request, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/locations/{locationId}/zones", request, token, success, problem);

        public static async Task<long> CreateSchedule(this HttpClient client, CreateScheduleCommand request, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/programs/{request.ProgramId}/schedules", request, token, success, problem);

        public static async Task<long> CreateSetPoint(this HttpClient client, long scheduleId, CreateSetPointCommand request, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/schedules/{scheduleId}/setPoints", request, token, success, problem);

        public static async Task ActivateProgram(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await PostWithNoContent(client, $"api/programs/{programId}/activate", token, success, problem);

        public static async Task ActivateSchedule(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await PostWithNoContent(client, $"api/schedules/{scheduleId}/activate", token, success, problem);

        public static async Task DeleteLocation(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/locations/{locationId}", token, success, problem);

        public static async Task<ZoneInfo[]> GetZones(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneInfo[]>(client, $"api/locations/{locationId}/zones", token, success, problem);

        public static async Task<Programs.Program[]> GetPrograms(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Programs.Program[]>(client, $"api/locations/{locationId}/programs", token, success, problem);

        public static async Task<ScheduleInfo[]> GetSchedules(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ScheduleInfo[]>(client, $"api/programs/{programId}/schedules", token, success, problem);

        public static async Task<SetPointInfo[]> GetSetPoints(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<SetPointInfo[]>(client, $"api/schedules/{scheduleId}/setPoints", token, success, problem);


        public static async Task<ZoneDetails> GetZoneDetails(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneDetails>(client, $"api/zones/{zoneId}", token, success, problem);

        public static async Task<Sensor[]> GetSensors(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Sensor[]>(client, $"api/zones/{zoneId}/sensors", token, success, problem);

        public static async Task UpdateSensor(this HttpClient client, UpdateSensorCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/sensors/{command.SensorId}", command, token, success, problem);

        public static async Task UpdateSchedule(this HttpClient client, UpdateScheduleCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/schedules/{command.ScheduleId}", command, token, success, problem);

        public static async Task UpdateProgram(this HttpClient client, UpdateProgramCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/programs/{command.ProgramId}", command, token, success, problem);

        public static async Task DeleteSensor(this HttpClient client, long sensorId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/sensors/{sensorId}", token, success, problem);

        public static async Task DeleteSetPoint(this HttpClient client, long setPointId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/setPoints/{setPointId}", token, success, problem);

        public static async Task DeleteSchedule(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/schedules/{scheduleId}", token, success, problem);

        public static async Task DeleteProgram(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/programs/{programId}", token, success, problem);

        public static async Task<ApiKey> GetApiKey(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ApiKey>(client, "api/users/apikey", token, success, problem);

        public static async Task CreateMeasurements(this HttpClient client, MeasurementCommand[] requests, string token)
            => await PostWithNoResponse(client, "api/measurements/", requests, token);

        public static async Task<Measurement[]> GetLatestMeasurements(this HttpClient client, long limit, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<Measurement[]>(client, $"api/measurements/latest?limit={limit}", token, success, problem);

        public static async Task UpdateUser(this HttpClient client, UpdateUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/users/{command.UserId}", command, token, success, problem);

        public static async Task UpdateCurrentUser(this HttpClient client, UpdateCurrentUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, $"api/users", command, token, success, problem);

        public static async Task ChangePassword(this HttpClient client, ChangePasswordCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, "api/users/password", command, token, success, problem);
    }
}
