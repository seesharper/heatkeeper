using System.Configuration;

namespace HeatKeeper.Server.WebApi.Tests;

public class ForecastTests : TestBase
{
    [Fact]
    public async Task ShouldGetForecast()
    {
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        var forecasts = await client.Get(new GetForeCastQuery(testLocation.LocationId), testLocation.Token);
        forecasts.Should().NotBeNull();
        forecasts.Should().NotBeEmpty();
    }
}