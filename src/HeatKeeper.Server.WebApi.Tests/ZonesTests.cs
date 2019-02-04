using System;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.WebApi.Tests.Transactions;
using HeatKeeper.Server.WebApi.Zones;
using LightInject;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
        public async Task TestShouldCreateZone()
        {
            var client = Factory.CreateClient();
            await client.PostAsync("api/locations", new JsonContent(new CreateLocationRequest("Home", "This is my home in Norway")));
            var response = await client.PostAsync("api/zones", new JsonContent(new CreateZoneRequest("Livingroom", "Livingroom", "Home")));
            response.EnsureSuccessStatusCode();
        }


    }
}
