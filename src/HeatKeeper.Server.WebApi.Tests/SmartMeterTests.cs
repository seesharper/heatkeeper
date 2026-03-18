using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using HeatKeeper.Server.SmartMeter;

namespace HeatKeeper.Server.WebApi.Tests;

public class SmartMeterTests : TestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task ShouldReturnSmartMeterReadingsAsServerSentEvents()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var token = testLocation.Token;
        var now = DateTime.UtcNow;

        // Create sensor by sending an initial measurement, then assign it to a zone
        const string sensorExternalId = "SMART_METER_TEST_SENSOR";
        await client.CreateMeasurements([
            new MeasurementCommand(sensorExternalId, MeasurementType.ActivePowerImport, RetentionPolicy.None, 0, now.AddMinutes(-1)),
        ], token);

        var unassigned = await client.GetUnassignedSensors(token);
        var sensor = unassigned.Single(s => s.ExternalId == sensorExternalId);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor.Id, testLocation.LivingRoomZoneId), token);

        // Submit all smart meter measurement types so LatestZoneMeasurements is populated
        await client.CreateMeasurements([
            new MeasurementCommand(sensorExternalId, MeasurementType.ActivePowerImport,          RetentionPolicy.None, 1500.0, now),
            new MeasurementCommand(sensorExternalId, MeasurementType.CurrentPhase1,              RetentionPolicy.None, 6.5,    now),
            new MeasurementCommand(sensorExternalId, MeasurementType.CurrentPhase2,              RetentionPolicy.None, 7.0,    now),
            new MeasurementCommand(sensorExternalId, MeasurementType.CurrentPhase3,              RetentionPolicy.None, 7.5,    now),
            new MeasurementCommand(sensorExternalId, MeasurementType.VoltageBetweenPhase1AndPhase2, RetentionPolicy.None, 230.0, now),
            new MeasurementCommand(sensorExternalId, MeasurementType.VoltageBetweenPhase1AndPhase3, RetentionPolicy.None, 231.0, now),
            new MeasurementCommand(sensorExternalId, MeasurementType.VoltageBetweenPhase2AndPhase3, RetentionPolicy.None, 232.0, now),
            new MeasurementCommand(sensorExternalId, MeasurementType.CumulativePowerImport,      RetentionPolicy.None, 150000, now),
        ], token);

        // Call the SSE endpoint and read the first event
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var request = new HttpRequestMessage(HttpMethod.Get, "api/smart-meter");
        request.Headers.Add("Authorization", $"Bearer {token}");

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/event-stream");

        var reading = await ReadFirstSseEvent<SmartMeterReadings>(response, cts.Token);

        reading.Should().NotBeNull();
        reading!.ActivePowerImport.Should().Be(1500.0);
        reading.CurrentPhase1.Should().Be(6.5);
        reading.CurrentPhase2.Should().Be(7.0);
        reading.CurrentPhase3.Should().Be(7.5);
        reading.VoltageBetweenPhase1AndPhase2.Should().Be(230.0);
        reading.VoltageBetweenPhase1AndPhase3.Should().Be(231.0);
        reading.VoltageBetweenPhase2AndPhase3.Should().Be(232.0);
        reading.CumulativePowerImport.Should().Be(150000);
    }

    [Fact]
    public async Task ShouldRequireAuthentication()
    {
        var client = Factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "api/smart-meter");
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<T?> ReadFirstSseEvent<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) break;
            if (line.StartsWith("data: "))
            {
                var json = line["data: ".Length..];
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
        }

        return default;
    }
}
