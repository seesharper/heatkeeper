using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Dashboard;
using HeatKeeper.Server.EnergyPriceAreas;
using HeatKeeper.Server.EnergyPriceAreas.Api;
using HeatKeeper.Server.EnergyPrices.Api;
using HeatKeeper.Server.Heaters.Api;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Mqtt;
using HeatKeeper.Server.Notifications;
using HeatKeeper.Server.Notifications.Api;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Programs.Api;
using HeatKeeper.Server.PushSubscriptions;
using HeatKeeper.Server.PushSubscriptions.Api;
using HeatKeeper.Server.QueryConsole;
using HeatKeeper.Server.Schedules.Api;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Sensors.Api;
using HeatKeeper.Server.SetPoints.Api;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Users.Api;
using HeatKeeper.Server.VATRates;
using HeatKeeper.Server.Version;
using HeatKeeper.Server.Zones;
using HeatKeeper.Server.Zones.Api;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;
using Xunit.Sdk;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static partial class HttpClientExtensions
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

        public static async Task<long> CreateUser(this HttpClient client, CreateUserCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/users", content, token, success, problem);

        public static async Task<long> CreateVATRate(this HttpClient client, PostVATRateCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/vat-rates", content, token, success, problem);

        public static async Task UpdateVATRate(this HttpClient client, PatchVATRateCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/vat-rates/{content.Id}", content, token, success, problem);

        public static async Task DeleteVATRate(this HttpClient client, long vatRateId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/vat-rates/{vatRateId}", token, success, problem);

        public static async Task<VATRateInfo[]> GetVATRates(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<VATRateInfo[]>(client, "api/vat-rates", token, success, problem);

        public static async Task<VATRateDetails> GetVATRateDetails(this HttpClient client, long vatRateId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<VATRateDetails>(client, $"api/vat-rates/{vatRateId}", token, success, problem);

        public static async Task<long> CreateEnergyPriceArea(this HttpClient client, PostEnergyPriceAreaCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/energy-price-areas", content, token, success, problem);

        public static async Task UpdateEnergyPriceArea(this HttpClient client, PatchEnergyPriceAreaCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/energy-price-areas/{content.Id}", content, token, success, problem);

        public static async Task DeleteEnergyPriceArea(this HttpClient client, long energyPriceAreaId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/energy-price-areas/{energyPriceAreaId}", token, success, problem);

        public static async Task<EnergyPriceAreaInfo[]> GetEnergyPriceAreas(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<EnergyPriceAreaInfo[]>(client, "api/energy-price-areas", token, success, problem);

        public static async Task<EnergyPriceAreaDetails> GetEnergyPriceAreaDetails(this HttpClient client, long energyPriceAreaId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<EnergyPriceAreaDetails>(client, $"api/energy-price-areas/{energyPriceAreaId}", token, success, problem);

        public static async Task ImportEnergyPrices(this HttpClient client, ImportEnergyPricesCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, "api/energy-prices/import", command, token, success, problem);

        public static async Task<long> CreateNotification(this HttpClient client, PostNotificationCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/users/{command.UserId}/notifications", command, token, success, problem);

        public static async Task<NotificationInfo[]> GetNotifications(this HttpClient client, long userId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<NotificationInfo[]>(client, $"api/users/{userId}/notifications", token, success, problem);

        public static async Task<NotificationDetails> GetNotificationDetails(this HttpClient client, long notificationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<NotificationDetails>(client, $"api/notifications/{notificationId}", token, success, problem);


        public static async Task<EnergyPrice[]> GetEnergyPrices(this HttpClient client, string dateToImport, long energyPriceAreaId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<EnergyPrice[]>(client, $"api/energy-prices?date={dateToImport}&energyPriceAreaId={energyPriceAreaId}", token, success, problem);

        public static async Task AssignLocationToUser(this HttpClient client, AssignLocationToUserCommand assignLocationToUserCommand, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/users/{assignLocationToUserCommand.UserId}/assignLocation", assignLocationToUserCommand, token, success, problem);

        public static async Task RemoveLocationFromUser(this HttpClient client, RemoveLocationFromUserCommand removeLocationFromUserCommand, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/users/{removeLocationFromUserCommand.UserId}/removeLocation", removeLocationFromUserCommand, token, success, problem);

        public static async Task<Table> ExecuteDatabaseQuery(this HttpClient client, string sql, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post<DatabaseQuery, Table>(client, $"api/queryconsole", new DatabaseQuery(sql), token, response => response.StatusCode.Should().Be(HttpStatusCode.OK), problem);

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

        public static async Task<UserInfo[]> GetAllUsers(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<UserInfo[]>(client, "api/users", token, success, problem);

        public static async Task<UserDetails> GetUserDetails(this HttpClient client, long userId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<UserDetails>(client, $"api/users/{userId}", token, success, problem);

        public static async Task<UserLocationAccess[]> GetUserLocationsAccess(this HttpClient client, long userId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<UserLocationAccess[]>(client, $"api/users/{userId}/locations-access", token, success, problem);

        public static async Task<DeadSensor[]> GetDeadSensors(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<DeadSensor[]>(client, "api/sensors/deadsensors", token, success, problem);

        public static async Task<UnassignedSensorInfo[]> GetUnassignedSensors(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<UnassignedSensorInfo[]>(client, "api/sensors", token, success, problem);

        public static async Task<UserInfo[]> GetUsersByLocation(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<UserInfo[]>(client, $"api/locations/{locationId}/users", token, success, problem);

        public static async Task<AppVersion> GetAppVersion(this HttpClient client)
            => await Get<AppVersion>(client, "api/version", string.Empty);

        public static async Task<DashboardLocation[]> GetDashboardLocations(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<DashboardLocation[]>(client, "api/dashboard/locations", token, success, problem);

        public static async Task<LocationDetails> GetLocationDetails(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<LocationDetails>(client, $"api/locations/{locationId}", token, success, problem);

        public static async Task<LocationTemperature[]> GetLocationTemperatures(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<LocationTemperature[]>(client, $"api/locations/{locationId}/temperatures", token, success, problem);

        public static async Task<ProgramDetails> GetProgramDetails(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ProgramDetails>(client, $"api/programs/{programId}", token, success, problem);

        public static async Task<ScheduleDetails> GetScheduleDetails(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ScheduleDetails>(client, $"api/schedules/{scheduleId}", token, success, problem);

        public static async Task<HeaterDetails> GetHeatersDetails(this HttpClient client, long heaterId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<HeaterDetails>(client, $"api/heaters/{heaterId}", token, success, problem);

        public static async Task<SensorDetails> GetSensorDetails(this HttpClient client, long sensorId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<SensorDetails>(client, $"api/sensors/{sensorId}", token, success, problem);

        public static async Task<ZoneInfo[]> GetZonesNotAssignedToSchedule(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneInfo[]>(client, $"api/schedules/{scheduleId}/zones", token, success, problem);

        public static async Task<SetPointDetails> GetSetPointDetails(this HttpClient client, long setPointId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<SetPointDetails>(client, $"api/setpoints/{setPointId}", token, success, problem);

        public static async Task RemoveUserFromLocation(this HttpClient client, long locationId, long userID, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/locations/{locationId}/users/{userID}", token, success, problem);

        public static async Task CreatePushSubscription(this HttpClient client, CreatePushSubscriptionCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await PostWithNoResponse(client, "api/pushsubscriptions", command, token, success, problem);




        public static async Task<string> CreateAndAuthenticateStandardUser(this HttpClient client)
        {
            var token = await client.AuthenticateAsAdminUser();

            var registerUserRequest = TestData.Users.StandardUser;
            await client.CreateUser(registerUserRequest, token);
            var authenticateResponse = await client.PostAuthenticateRequest(registerUserRequest.Email, registerUserRequest.NewPassword);
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

        public static async Task<long> CreateHeater(this HttpClient client, CreateHeaterCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/zones/{content.ZoneId}/heaters", content, token, success, problem);

        public static async Task<long> CreateProgram(this HttpClient client, CreateLocationCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Post(client, $"api/programs", content, token, success, problem);

        public static async Task UpdateLocation(this HttpClient client, UpdateLocationCommand content, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/locations/{locationId}", content, token, success, problem);

        public static async Task UpdateZone(this HttpClient client, UpdateZoneCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/zones/{content.ZoneId}", content, token, success, problem);

        public static async Task UpdateSetPoint(this HttpClient client, UpdateSetPointCommand content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/setPoints/{content.SetPointId}", content, token, success, problem);

        public static async Task<LocationInfo[]> GetLocations(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<LocationInfo[]>(client, "api/locations", token, success, problem);

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

        public static async Task<long> Post<TContent>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
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
                problem.Should().NotBeNull($"There was a problem handling the request {response.RequestMessage.RequestUri}and this was not not handled in the calling test method. The status code was ({(int)response.StatusCode}) {response.StatusCode}");
                var problemDetails = await response.ContentAs<ProblemDetails>();
                problem(problemDetails);
            }

            return response;
        }

        private static async Task Patch<TContent>(HttpClient client, string uri, TContent content, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        {
            success ??= (response) => response.StatusCode.Should().Be(HttpStatusCode.NoContent);

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

        public static async Task<ProgramInfo[]> GetPrograms(this HttpClient client, long locationId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ProgramInfo[]>(client, $"api/locations/{locationId}/programs", token, success, problem);

        public static async Task<HeaterInfo[]> GetHeaters(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Get<HeaterInfo[]>(client, $"api/zones/{zoneId}/heaters", token, success, problem);

        public static async Task<ScheduleInfo[]> GetSchedules(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ScheduleInfo[]>(client, $"api/programs/{programId}/schedules", token, success, problem);

        public static async Task<SetPointInfo[]> GetSetPoints(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<SetPointInfo[]>(client, $"api/schedules/{scheduleId}/setPoints", token, success, problem);


        public static async Task<ZoneDetails> GetZoneDetails(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneDetails>(client, $"api/zones/{zoneId}", token, success, problem);

        public static async Task<ZoneInsights> GetZoneInsights(this HttpClient client, long zoneId, TimeRange range, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ZoneInsights>(client, $"api/zones/{zoneId}/insights?Range={range}", token, success, problem);


        public static async Task<SensorInfo[]> GetSensors(this HttpClient client, long zoneId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<SensorInfo[]>(client, $"api/zones/{zoneId}/sensors", token, success, problem);

        public static async Task UpdateSensor(this HttpClient client, UpdateSensorCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/sensors/{command.SensorId}", command, token, success, problem);

        public static async Task AssignZoneToSensor(this HttpClient client, AssignZoneToSensorCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/sensors/{command.SensorId}/assignZone", command, token, success, problem);

        public static async Task RemovedZoneFromSensor(this HttpClient client, RemoveZoneFromSensorCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/sensors/{command.SensorId}/removeZone", command, token, success, problem);

        // public static async Task RemoveSensorFromZone(this HttpClient client, AssignSensorToZoneCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
        //    => await Delete(client, $"api/zones/{command.ZoneId}/sensors", command, token, success, problem);


        public static async Task UpdateSchedule(this HttpClient client, UpdateScheduleCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/schedules/{command.ScheduleId}", command, token, success, problem);

        public static async Task UpdateHeater(this HttpClient client, UpdateHeaterCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/heaters/{command.HeaterId}", command, token, success, problem);

        public static async Task UpdateProgram(this HttpClient client, UpdateProgramCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Patch(client, $"api/programs/{command.ProgramId}", command, token, success, problem);

        public static async Task DeleteSensor(this HttpClient client, long sensorId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/sensors/{sensorId}", token, success, problem);

        public static async Task DeleteSetPoint(this HttpClient client, long setPointId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/setPoints/{setPointId}", token, success, problem);

        public static async Task DeleteSchedule(this HttpClient client, long scheduleId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/schedules/{scheduleId}", token, success, problem);

        public static async Task DeleteHeater(this HttpClient client, long heaterId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
           => await Delete(client, $"api/heaters/{heaterId}", token, success, problem);

        public static async Task DeleteProgram(this HttpClient client, long programId, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Delete(client, $"api/programs/{programId}", token, success, problem);

        public static async Task<ApiKey> GetApiKey(this HttpClient client, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Get<ApiKey>(client, "api/users/apikey", token, success, problem);

        public static async Task CreateMeasurements(this HttpClient client, MeasurementCommand[] requests, string token)
            => await PostWithNoResponse(client, "api/measurements/", requests, token);

        public static async Task PublishMqttMessage(this HttpClient client, PublishMqttMessageCommand command, string token)
            => await PostWithNoResponse(client, "api/mqtt", command, token);

        public static async Task UpdateUser(this HttpClient client, UpdateUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null)
            => await Patch(client, $"api/users/{command.UserId}", command, token, success, problem);

        public static async Task UpdateCurrentUser(this HttpClient client, UpdateCurrentUserCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, $"api/users", command, token, success, problem);

        public static async Task ChangePassword(this HttpClient client, ChangePasswordCommand command, string token, Action<HttpResponseMessage> success = null, Action<ProblemDetails> problem = null) =>
            await Patch(client, "api/users/password", command, token, success, problem);


        public static async Task CreateLivingRoomTemperatureMeasurements(this HttpClient client, int count, TimeSpan interval, RetentionPolicy retentionPolicy, string token)
        {
            var created = TestData.Clock.Today;
            List<MeasurementCommand> commands = new();
            for (var i = 0; i < count; i++)
            {
                double[] temperatures = [20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30];
                var randomTemperatureValue = Random.Shared.GetItems(temperatures, 1).Single();
                MeasurementCommand command = new MeasurementCommand(TestData.Sensors.LivingRoomSensor, MeasurementType.Temperature, retentionPolicy, randomTemperatureValue, created);
                commands.Add(command);
                created = created.Add(interval);
            }
            await client.CreateMeasurements(commands.ToArray(), token);
        }
    }
}
