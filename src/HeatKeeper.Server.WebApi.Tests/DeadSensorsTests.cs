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
        var testApplication = await Factory.CreateTestLocation();

        var deadSensors = await client.GetDeadSensors(testApplication.Token);

        deadSensors.Should().BeEmpty();

        var connection = Factory.Services.GetService<IDbConnection>();

        await connection.ExecuteAsync("UPDATE Sensors SET LastSeen = @LastSeen", new { @LastSeen = DateTime.UtcNow.AddHours(-14) });

        deadSensors = await client.GetDeadSensors(testApplication.Token);

        deadSensors.Length.Should().Be(1);

    }
}