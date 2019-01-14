using System.Threading.Tasks;
using HeatKeeper.Server.Measurements;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class MeasurementsTests : TestBase
    {
        private const string SensorID = "S1";

        public MeasurementsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task ShouldCreateMeasurement()
        {
            var createMeasurementRequest = new CreateMeasurementRequest(SensorID, MeasurementType.Temperature, 23.7);
            var measurements = new []{createMeasurementRequest};
            var client = Factory.CreateClient();
            await client.PostAsync("api/measurements", new JsonContent(measurements));
        }
    }
}