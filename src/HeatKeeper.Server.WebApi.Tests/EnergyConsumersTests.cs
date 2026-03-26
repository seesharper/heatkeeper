using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Locations.Api;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Sensors.Api;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class EnergyConsumersTests : TestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string SensorExternalId = "ENERGY_CONSUMER_TEST_SENSOR";

    [Fact]
    public async Task ShouldReturnEnergyConsumersAsServerSentEvents()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var now = DateTime.UtcNow;

        await SetupSensorWithMeasurement(client, testLocation, 1500.0, now);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var consumers = await ReadFirstSseEvent<EnergyConsumer[]>(client, $"api/locations/{testLocation.LocationId}/energy-consumers", testLocation.Token, cts.Token);

        consumers.Should().HaveCount(1);
        consumers![0].ActivePowerImport.Should().Be(1500.0);
        consumers[0].SensorName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldExcludeZeroConsumersWhenShowAllIsFalse()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var now = DateTime.UtcNow;

        await SetupSensorWithMeasurement(client, testLocation, 0.0, now);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var consumers = await ReadFirstSseEvent<EnergyConsumer[]>(client, $"api/locations/{testLocation.LocationId}/energy-consumers?showAll=false", testLocation.Token, cts.Token);

        consumers.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldIncludeZeroConsumersWhenShowAllIsTrue()
    {
        var testLocation = await Factory.CreateTestLocation();
        var client = testLocation.HttpClient;
        var now = DateTime.UtcNow;

        await SetupSensorWithMeasurement(client, testLocation, 0.0, now);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var consumers = await ReadFirstSseEvent<EnergyConsumer[]>(client, $"api/locations/{testLocation.LocationId}/energy-consumers?showAll=true", testLocation.Token, cts.Token);

        consumers.Should().HaveCount(1);
        consumers![0].ActivePowerImport.Should().Be(0.0);
    }

    private static async Task SetupSensorWithMeasurement(HttpClient client, TestLocation testLocation, double value, DateTime now)
    {
        await client.CreateMeasurements([
            new MeasurementCommand(SensorExternalId, MeasurementType.ActivePowerImport, RetentionPolicy.None, 0, now.AddMinutes(-1)),
        ], testLocation.Token);

        var unassigned = await client.GetUnassignedSensors(testLocation.Token);
        var sensor = unassigned.Single(s => s.ExternalId == SensorExternalId);
        await client.AssignZoneToSensor(new AssignZoneToSensorCommand(sensor.Id, testLocation.LivingRoomZoneId), testLocation.Token);

        await client.CreateMeasurements([
            new MeasurementCommand(SensorExternalId, MeasurementType.ActivePowerImport, RetentionPolicy.None, value, now),
        ], testLocation.Token);
    }

    private static async Task<T?> ReadFirstSseEvent<T>(HttpClient client, string url, string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", $"Bearer {token}");

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/event-stream");

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
