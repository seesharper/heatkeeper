using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DbReader;
using FluentAssertions;
using HeatKeeper.Server.Sensors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class DeadSensorsTests : TestBase
{
    [Fact]
    public async Task ShouldGetDeadSensors()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();
        var apiKey = await client.GetApiKey(token);

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var livingRoomZoneId = await client.CreateZone(locationId, TestData.Zones.LivingRoom, token);

        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

        var sensors = await client.GetSensors(livingRoomZoneId, token);

        var livingroomSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.LivingRoomSensor);
        var outsideSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.OutsideSensor);

        await client.UpdateSensor(new UpdateSensorCommand() { SensorId = livingroomSensor.Id, Name = livingroomSensor.Name, Description = livingroomSensor.Description, ZoneId = livingRoomZoneId }, token);


        await client.CreateMeasurements(TestData.TemperatureMeasurementRequests, apiKey.Token);

        var deadSensors = await client.GetDeadSensors(token);

        deadSensors.Should().BeEmpty();

        var connection = Factory.Services.GetService<IDbConnection>();

        await connection.ExecuteAsync("UPDATE Sensors SET LastSeen = @LastSeen", new { @LastSeen = DateTime.UtcNow.AddHours(-14) });

        deadSensors = await client.GetDeadSensors(token);

        deadSensors.Length.Should().Be(1);

    }
}