using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using HeatKeeper.Server.Programs.Api;
using HeatKeeper.Server.Schedules.Api;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Sensors.Api;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Users.Api;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.WebApi.Tests;

public static class TestApplicationExtensions
{
    public static async Task<HttpClient> CreateAuthenticatedClient<TEntryPoint>(this TestApplication<TEntryPoint> testApplication) where TEntryPoint : class
    {
        var client = testApplication.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await client.AuthenticateAsAdminUser()}");
        // new AuthenticationHeaderValue("Bearer", this.bearerToken);
        return client;
    }

    public static async Task<TestLocation> CreateTestLocation<TEntryPoint>(this TestApplication<TEntryPoint> testApplication) where TEntryPoint : class
    {
        var client = testApplication.CreateClient();

        var token = await client.AuthenticateAsAdminUser();

        var vatRateId = await client.CreateVATRate(TestData.VatRates.Vat25, token);
        var priceAreaId = await client.CreateEnergyPriceArea(TestData.EnergyPriceAreas.Norway3, token);


        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);
        var kitchenZoneId = await client.CreateZone(locationId, TestData.Zones.Kitchen, token);

        var livingRoomHeaterId1 = await client.CreateHeater(TestData.Heaters.LivingRoomHeater1(livingRoomZoneId), token);
        var livingRoomHeaterId2 = await client.CreateHeater(TestData.Heaters.LivingRoomHeater1(livingRoomZoneId), token);
        var kitchenHeaterId = await client.CreateHeater(TestData.Heaters.KitchenHeater(kitchenZoneId), token);

        await client.UpdateLocation(new UpdateLocationCommand(locationId, TestData.Locations.Home.Name, TestData.Locations.Home.Description, null, livingRoomZoneId), locationId, token);

        await client.AssignLocationToUser(new AssignLocationToUserCommand(1, locationId), token);

        // Create measurements in the living room zone and outside zone
        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

        var unassignedSensors = await client.GetUnassignedSensors(token);
        var livingRoomSensor = unassignedSensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(livingRoomSensor.Id, livingRoomZoneId), token);

        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, token);

        var normalProgramId = await client.CreateProgram(TestData.Programs.Normal(locationId), token);

        await client.ActivateProgram(normalProgramId, token);

        var createScheduleCommand = new CreateScheduleCommand(normalProgramId, "DayTime", "0 15,18,21 * * *");

        var scheduleId = await client.CreateSchedule(createScheduleCommand, token);

        await client.ActivateSchedule(scheduleId, token);

        var createSetPointCommand = new CreateSetPointCommand(scheduleId, livingRoomZoneId, 20, 2);

        var setPointId = await client.CreateSetPoint(scheduleId, createSetPointCommand, token);

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return new TestLocation(client, testApplication.Services, token, locationId, livingRoomZoneId, livingRoomSensor.Id, kitchenZoneId, normalProgramId, scheduleId, setPointId, livingRoomHeaterId1, livingRoomHeaterId2, kitchenHeaterId, vatRateId, priceAreaId);
    }
}

public record TestLocation(HttpClient HttpClient, IServiceProvider ServiceProvider, string Token, long LocationId, long LivingRoomZoneId, long LivingRoomSensorId, long KitchenZoneId, long NormalProgramId, long ScheduleId, long LivingRoomSetPointId, long LivingRoomHeaterId1, long LivingRoomHeaterId2, long KitchenHeaterId, long VATRateId, long PriceAreaId)
{
    public async Task DeleteAllMeasurements()
    {
        var connection = ServiceProvider.GetRequiredService<IDbConnection>();
        await connection.ExecuteAsync("DELETE FROM Measurements");
    }
}