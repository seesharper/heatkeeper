using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HeatKeeper.Server.Measurements;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class MeasurementsTests : TestBase
    {
        public MeasurementsTests()
        {
        }

        [Fact]
        public async Task ShouldCreateMeasurementUsingApiKey()
        {
            var client = Factory.CreateClient();
            var token = await client.AuthenticateAsAdminUser();
            var apiKey = await client.GetApiKey(token);

            var response = await client.CreateMeasurement(TestData.TemperatureMeasurementRequests, apiKey);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.EnsureSuccessStatusCode();
        }
    }
}
