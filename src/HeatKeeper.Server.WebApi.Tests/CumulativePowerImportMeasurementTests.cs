using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;


public class CumulativePowerImportMeasurementTests : TestBase
{

    [Fact]
    public async Task Method()
    {
        var now = DateTime.Now;

        List<MeasurementCommand> commands = new List<MeasurementCommand>();
        for (int i = 0; i < 24; i++)
        {
            var startDate = new DateTime(now.Year, now.Month, now.Day, i, 0, 0, DateTimeKind.Utc);
            var command = new MeasurementCommand(TestData.Sensors.PowerMeter, MeasurementType.CumulativePowerImport, RetentionPolicy.None, 150000 + (i * 100), startDate);
            commands.Add(command);
        }

        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();
        var apiKey = await client.GetApiKey(token);

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var zoneId = await client.CreateZone(locationId, TestData.Zones.PowerMeter, token);

        await client.CreateMeasurements(commands.ToArray(), apiKey.Token);

        var sensors = await client.GetSensors(zoneId, token);

        var powerMeterSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);

        await client.UpdateSensor(new UpdateSensorCommand() { SensorId = powerMeterSensor.Id, Name = powerMeterSensor.Name, Description = powerMeterSensor.Description, ZoneId = zoneId }, token);

        await client.CreateMeasurements(TestData.CumulativeMeasurementsRequests, apiKey.Token);

    }




    [Fact]
    public async Task ShouldCreateCumulativeMeasurements()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();
        var apiKey = await client.GetApiKey(token);

        var locationId = await client.CreateLocation(TestData.Locations.Home, token);
        var zoneId = await client.CreateZone(locationId, TestData.Zones.PowerMeter, token);

        await client.CreateMeasurements(TestData.CumulativeMeasurementsRequests, apiKey.Token);
        var sensors = await client.GetSensors(zoneId, token);

        var powerMeterSensor = sensors.Single(s => s.ExternalId == TestData.Sensors.PowerMeter);

        await client.UpdateSensor(new UpdateSensorCommand() { SensorId = powerMeterSensor.Id, Name = powerMeterSensor.Name, Description = powerMeterSensor.Description, ZoneId = zoneId }, token);

        await client.CreateMeasurements(TestData.CumulativeMeasurementsRequests, apiKey.Token);
    }
}