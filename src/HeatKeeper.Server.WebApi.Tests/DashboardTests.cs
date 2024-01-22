using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class DashboardTests : TestBase
{

    [Fact]
    public async Task ShouldGetDashboardTemperatures()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        await testLocation.AddLivingRoomMeasurement(20);
        var dashboardTemperatures = await client.GetDashboardTemperatures(testLocation.Token);
        dashboardTemperatures.Should().HaveCount(1);
        dashboardTemperatures[0].Temperature.Should().Be(20);
        dashboardTemperatures[0].Humidity.Should().Be(39.3);
    }
}