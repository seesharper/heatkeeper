using System.Threading;
using CQRS.AspNet.Testing;
using CsvHelper;
using HeatKeeper.Server.Locations.Api;
using Moq;

namespace HeatKeeper.Server.WebApi.Tests.Lighting;

public class OutDoorLightingTests : TestBase
{

    [Fact]
    public async Task ShouldTurnLightsOnWhenSunsets()
    {
        var fakeTimeProvider = Factory.UseFakeTimeProvider(TestData.Clock.Today);
        var mock = Factory.MockQueryHandler<GetLocationCoordinatesQuery, LocationCoordinates[]>();
        mock.Setup(m => m.HandleAsync(It.IsAny<GetLocationCoordinatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new LocationCoordinates(1, "Home", 59.9111, 10.7528)
            }); 
        var testLocation = await Factory.CreateTestLocation();


    }
}