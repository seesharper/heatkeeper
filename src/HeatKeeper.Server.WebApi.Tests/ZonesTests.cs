using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public class ZonesTests : TestBase
    {
        public ZonesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }




        [Fact]
        public async Task ShouldGetZonesForLocation()
        {
            var client = Factory.CreateClient();
            await client.GetZones(12);

        }


    }
}
