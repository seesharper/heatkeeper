using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.TimeZones;
using Xunit;

namespace HeatKeeper.Server.WebApi.Tests;

public class TimeZonesTests : TestBase
{
    [Fact]
    public async Task ShouldReturnTimeZones()
    {
        var client = Factory.CreateClient();
        var token = await client.AuthenticateAsAdminUser();

        var timeZones = await client.GetTimeZones(token);

        timeZones.Should().NotBeEmpty();
        timeZones.Should().Contain(tz => tz.Id == "Europe/Oslo");
    }
}
